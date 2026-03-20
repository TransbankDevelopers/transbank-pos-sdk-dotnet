using System;
using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSAutoservicioCloseE2ETests : POSAutoservicioE2ETestBase
    {
        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(CloseWithDataVoucherResponsePayload);

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.Contains("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$20.000", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithoutVoucher_WhenThereAreCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|0";
            const string responsePayload = "0510|00|597029414300|IM750164|";

            var task = _pos.Close(sendVoucher: false);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreNoCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(CloseWithoutDataVoucherResponsePayload);

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.False(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.DoesNotContain("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$0", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithoutVoucher_WhenThereAreNoCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|0";
            const string responsePayload = "0510|00|597029414300|IM750164|";

            var task = _pos.Close(sendVoucher: false);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldReturnEmptyPrintingFieldAndEmptyRawVoucher_WhenVoucherIsMissing()
        {
            const string expectedCommandPayload = "0500|0";
            const string responsePayload = "0510|00|597029414300|IM750164|";

            var task = _pos.Close(sendVoucher: false);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldReturnEmptyPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsInvalid()
        {
            const string expectedCommandPayload = "0500|1";
            const string invalidRawVoucher = "CIERRE_INVALIDO";
            string responsePayload = $"0510|00|597029414300|IM750164|{invalidRawVoucher}";

            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.Equal(invalidRawVoucher, response.RawVoucher);
            AssertEmptyPrintingField(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldSegmentPrintingFieldAndPreserveRawVoucher_WhenVoucherLengthIsMultipleOf40()
        {
            const string expectedCommandPayload = "0500|1";
            string voucherLineOne = new string('C', 40);
            string voucherLineTwo = new string('D', 40);
            string validRawVoucher = voucherLineOne + voucherLineTwo;
            string responsePayload = $"0510|00|597029414300|IM750164|{validRawVoucher}";

            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.Equal(validRawVoucher, response.RawVoucher);
            Assert.Equal(2, response.PrintingField.Count);
            Assert.Equal(voucherLineOne, response.PrintingField[0]);
            Assert.Equal(voucherLineTwo, response.PrintingField[1]);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }
    }
}
