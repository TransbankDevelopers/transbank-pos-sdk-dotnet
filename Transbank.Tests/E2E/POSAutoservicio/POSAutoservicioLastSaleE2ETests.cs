using System;
using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Xunit;

namespace Transbank.Tests.E2E.POSAutoservicio
{
    public class POSAutoservicioLastSaleE2ETests : POSAutoservicioE2ETestBase
    {
        [Fact]
        public async Task LastSale_ShouldParseApprovedDebitResponseWithoutVoucher_WhenThereIsALastSale()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|10032026|331|P |12032026|171142";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;
            string lastSaleResponseText = response.ToString();

            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("574062", response.AuthorizationCode);
            Assert.Equal(1000, response.Amount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(56, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(new DateTime(2026, 3, 10), response.AccountingDate);
            Assert.Equal("331", response.AccountNumber);
            Assert.Equal("P", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 12, 17, 11, 42), response.RealDate);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            Assert.Equal(-1, response.InstallmentsType);
            Assert.Equal(-1, response.InstallmentsNumber);
            Assert.Equal(-1, response.InstallmentsAmount);
            Assert.Equal(string.Empty, response.InstallmentsTypeDescription);
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:0", lastSaleResponseText);
            Assert.Contains("Card Type: DB", lastSaleResponseText);
            Assert.Contains("Card Brand: P", lastSaleResponseText);
            Assert.Contains("Installments Type: -1", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task LastSale_ShouldParseAccountingDate_WhenDateIsValid()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|10032026|331|P |12032026|171142";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Equal(new DateTime(2026, 3, 10), response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldReturnMinValueAccountingDate_WhenDateIsInvalid()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|abc|331|P |12032026|171142";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Equal(DateTime.MinValue, response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldReturnNullAccountingDate_WhenDateIsMissing()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Null(response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedDebitResponseWithVoucher_WhenVoucherIsRequested()
        {
            const string expectedCommandPayload = "0250|1";
            var task = _pos.LastSale(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(LastSaleDebitWithVoucherResponsePayload);

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414303, "IM750164");
            AssertSaleFields(response, "123456", "912108", 1000, 3331, 68, "DB", "331", "P", new DateTime(2026, 3, 19, 10, 24, 38));
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0250|1";
            var task = _pos.LastSale(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendFragmentedResponseAndAssertPending(task, LastSaleCreditWithVoucherResponsePayload);

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "575354", 10000, 6590, 34, "CR", string.Empty, "VI", new DateTime(2026, 3, 17, 11, 50, 6));
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
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "575354", 10000, 6590, 34, "CR", string.Empty, "VI", new DateTime(2026, 3, 17, 11, 50, 6));
            Assert.Null(response.AccountingDate);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseNoSaleResponse_WhenThereIsNoLastSale()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|11|597029414300|IM750164";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            var response = await task;
            string lastSaleResponseText = response.ToString();

            Assert.NotNull(response);
            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(11, response.ResponseCode);
            Assert.False(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal(string.Empty, response.Ticket);
            Assert.Equal(string.Empty, response.AuthorizationCode);
            Assert.Equal(-1, response.Amount);
            Assert.Equal(-1, response.Last4Digits);
            Assert.Equal(-1, response.OperationNumber);
            Assert.Equal(string.Empty, response.CardType);
            Assert.Equal(string.Empty, response.CardBrand);
            Assert.Null(response.AccountingDate);
            Assert.Null(response.RealDate);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            Assert.Equal(-1, response.InstallmentsType);
            Assert.Equal(-1, response.InstallmentsNumber);
            Assert.Equal(-1, response.InstallmentsAmount);
            Assert.Equal(string.Empty, response.InstallmentsTypeDescription);
            AssertBaseResponseText(lastSaleResponseText, "0260", 11);
            AssertFinalAckWritten(2);
        }
    }
}
