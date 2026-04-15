using System;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Xunit;

namespace Transbank.Tests.E2E.POSAutoservicio
{
    public class POSAutoservicioMultiCodeSaleE2ETests : POSAutoservicioE2ETestBase
    {
        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedDebitResponseWithVoucher()
        {
            const string expectedCommandPayload = "0270|1000|123456|1|0|597029414303";

            var task = _pos.MultiCodeSale(1000, "123456", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(MultiCodeSaleDebitWithVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string multiCodeSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0271", 0, success: true, 597029414303, "IM750164");
            AssertSaleFields(response, "123456", "475618", 1000, 3331, 62, "DB", "331", "P", new DateTime(2026, 3, 18, 17, 10, 40));
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedDebitResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0270|1000|123456|0|0|597029414303";
            const string responsePayload = "0271|00|597029414303|IM750164|123456|673501|1000|3331|63|DB|00-00-00|331|P |18032026|171113|597012345678";

            var task = _pos.MultiCodeSale(1000, "123456", 597029414303);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0271", 0, success: true, 597029414303, "IM750164");
            AssertSaleFields(response, "123456", "673501", 1000, 3331, 63, "DB", "331", "P", new DateTime(2026, 3, 18, 17, 11, 13));
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0270|10000|123456|1|0|597029414303";

            var task = _pos.MultiCodeSale(10000, "123456", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendFragmentedResponseAndAssertPending(task, MultiCodeSaleCreditWithVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string multiCodeSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0271", 0, success: true, 597029414303, "IM750164");
            AssertSaleFields(response, "123456", "194937", 10000, 6590, 64, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 17, 11, 53));
            Assert.Null(response.AccountingDate);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0270|10000|123456|0|0|597029414303";
            const string responsePayload = "0271|00|597029414303|IM750164|123456|785992|10000|6590|65|CR|||VI|18032026|171232|597012345678||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.MultiCodeSale(10000, "123456", 597029414303);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0271", 0, success: true, 597029414303, "IM750164");
            AssertSaleFields(response, "123456", "785992", 10000, 6590, 65, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 17, 12, 32));
            Assert.Null(response.AccountingDate);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldReturnEmptyPrintingFieldAndEmptyRawVoucher_WhenVoucherIsMissing()
        {
            const string expectedCommandPayload = "0270|1000|123456|0|0|597029414303";
            const string responsePayload = "0271|00|597029414303|IM750164|123456|673501|1000|3331|63|DB|00-00-00|331|P |18032026|171113|597012345678";

            var task = _pos.MultiCodeSale(1000, "123456", 597029414303);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Equal(597012345678, response.CommerceProviderCode);
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldReturnEmptyPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsInvalid()
        {
            const string expectedCommandPayload = "0270|1000|123456|1|0|597029414303";
            const string invalidRawVoucher = "MULTICODIGO_INVALIDO";
            string responsePayload = $"0271|00|597029414303|IM750164|123456|475618|1000|3331|62|DB|00-00-00|331|P |18032026|171040|597012345678|{invalidRawVoucher}";

            var task = _pos.MultiCodeSale(1000, "123456", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal(invalidRawVoucher, response.RawVoucher);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldSegmentPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsMultipleOf40()
        {
            const string expectedCommandPayload = "0270|1000|123456|1|0|597029414303";
            string voucherLineOne = new string('M', 40);
            string voucherLineTwo = new string('N', 40);
            string validRawVoucher = voucherLineOne + voucherLineTwo;
            string responsePayload = $"0271|00|597029414303|IM750164|123456|475618|1000|3331|62|DB|00-00-00|331|P |18032026|171040|597012345678|{validRawVoucher}";

            var task = _pos.MultiCodeSale(1000, "123456", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal(validRawVoucher, response.RawVoucher);
            Assert.Equal(597012345678, response.CommerceProviderCode);
            Assert.Equal(2, response.PrintingField.Count);
            Assert.Equal(voucherLineOne, response.PrintingField[0]);
            Assert.Equal(voucherLineTwo, response.PrintingField[1]);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }
    }
}
