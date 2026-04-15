using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.CommonResponses;
using Transbank.Tests.Helpers;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoCoreE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IT750050";

            var task = _pos.LoadKeys();

            AssertSentCommand(expectedCommandPayload);

            SendResponse(responsePayload);

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            Assert.NotNull(response);
            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            AssertBaseResponseText(loadKeysResponseText, "0810", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Poll_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0100";

            var task = _pos.Poll();

            AssertSentCommand(expectedCommandPayload);

            SendAck();

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

    }
}
