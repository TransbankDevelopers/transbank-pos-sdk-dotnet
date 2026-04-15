using Transbank.Responses.CommonResponses;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoRefundE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async System.Threading.Tasks.Task Refund_ShouldParseDeniedDebitResponse()
        {
            const string expectedCommandPayload = "1200|143|";

            var task = _pos.Refund(143);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(RefundDebitDeniedResponsePayload);

            RefundResponse response = await task;
            string refundResponseText = response.ToString();

            Assert.Equal("1210", response.FunctionCode);
            Assert.Equal(21, response.ResponseCode);
            Assert.Equal("Anulación no permitida", response.ResponseMessage);
            Assert.False(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal(string.Empty, response.AuthorizationCode);
            Assert.Equal(143, response.OperationID);
            AssertBaseResponseText(refundResponseText, "1210", 21);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async System.Threading.Tasks.Task Refund_ShouldParseApprovedCreditResponse()
        {
            const string expectedCommandPayload = "1200|142|";

            var task = _pos.Refund(142);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(RefundCreditApprovedResponsePayload);

            RefundResponse response = await task;
            string refundResponseText = response.ToString();

            Assert.Equal("1210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal("Aprobado", response.ResponseMessage);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal("162529", response.AuthorizationCode);
            Assert.Equal(142, response.OperationID);
            AssertBaseResponseText(refundResponseText, "1210", 0);
            AssertFinalAckWritten(2);
        }
    }
}
