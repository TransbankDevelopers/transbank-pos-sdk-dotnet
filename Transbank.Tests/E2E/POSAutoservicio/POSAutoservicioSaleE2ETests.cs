using System;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Xunit;

namespace Transbank.Tests.E2E.POSAutoservicio
{
    public class POSAutoservicioSaleE2ETests : POSAutoservicioE2ETestBase
    {
        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|1|0";
            var task = _pos.Sale(1000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(SaleDebitWithVoucherResponsePayload);

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "547545", 1000, 3331, 55, "DB", "331", "P", new DateTime(2026, 3, 18, 12, 32, 30));
            Assert.Null(response.AccountingDate);
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, null, null, null, string.Empty);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|700527|1000|3331|56|DB|00-00-00|331|P |18032026|123307";

            var task = _pos.Sale(1000, "123456");

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "700527", 1000, 3331, 56, "DB", "331", "P", new DateTime(2026, 3, 18, 12, 33, 7));
            Assert.Null(response.AccountingDate);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, null, null, null, string.Empty);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|1|0";
            var task = _pos.Sale(10000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendFragmentedResponseAndAssertPending(task, SaleCreditWithVoucherResponsePayload);

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "316557", 10000, 6590, 57, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 12, 34, 29));
            Assert.Null(response.AccountingDate);
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|776549|10000|6590|58|CR|||VI|18032026|123506||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.Sale(10000, "123456");

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "776549", 10000, 6590, 58, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 12, 35, 6));
            Assert.Null(response.AccountingDate);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldReturnEmptyPrintingFieldAndEmptyRawVoucher_WhenVoucherIsMissing()
        {
            const string expectedCommandPayload = "0200|1000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|700527|1000|3331|56|DB|00-00-00|331|P |18032026|123307";

            var task = _pos.Sale(1000, "123456");

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldReturnEmptyPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsInvalid()
        {
            const string expectedCommandPayload = "0200|1000|123456|1|0";
            const string invalidRawVoucher = "VOUCHER_INVALIDO";
            string responsePayload = $"0210|00|597029414300|IM750164|123456|547545|1000|3331|55|DB|00-00-00|331|P |18032026|123230|{invalidRawVoucher}";

            var task = _pos.Sale(1000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            Assert.Equal(invalidRawVoucher, response.RawVoucher);
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldSegmentPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsMultipleOf40()
        {
            const string expectedCommandPayload = "0200|1000|123456|1|0";
            string voucherLineOne = new string('A', 40);
            string voucherLineTwo = new string('B', 40);
            string validRawVoucher = voucherLineOne + voucherLineTwo;
            string responsePayload = $"0210|00|597029414300|IM750164|123456|547545|1000|3331|55|DB|00-00-00|331|P |18032026|123230|{validRawVoucher}";

            var task = _pos.Sale(1000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            Assert.Equal(validRawVoucher, response.RawVoucher);
            Assert.Equal(2, response.PrintingField.Count);
            Assert.Equal(voucherLineOne, response.PrintingField[0]);
            Assert.Equal(voucherLineTwo, response.PrintingField[1]);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }
    }
}
