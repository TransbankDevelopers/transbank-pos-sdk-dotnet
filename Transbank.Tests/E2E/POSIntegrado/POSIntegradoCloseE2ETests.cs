using System.Threading.Tasks;
using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests.E2E.POSIntegrado
{
    public class POSIntegradoCloseE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task Close_ShouldParseApprovedResponse()
        {
            const string expectedCommandPayload = "0500||";

            var task = _pos.Close();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(CloseResponsePayload);

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.Equal("0510", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal("Aprobado", response.ResponseMessage);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }
    }
}
