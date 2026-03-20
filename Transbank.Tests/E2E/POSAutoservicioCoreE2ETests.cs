using System;
using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Responses.CommonResponses;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSAutoservicioCoreE2ETests : POSAutoservicioE2ETestBase
    {
        [Fact]
        public async Task Poll_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0100";

            var task = _pos.Poll();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

        [Fact]
        public async Task Initialization_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0070";

            var task = _pos.Initialization();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

        [Fact]
        public async Task InitializationResponse_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0080";
            const string responsePayload = "1080|90|03022026|111543";

            var task = _pos.InitializationResponse();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            InitializationResponse response = await task;
            string initializationResponseText = response.ToString();

            AssertBasicResponse(response, "1080", 90, success: true, 0, null);
            Assert.True(response.Success);
            Assert.Equal(new DateTime(2026, 2, 3, 11, 15, 43), response.RealDate);
            AssertBaseResponseText(initializationResponseText, "1080", 90);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            AssertBasicResponse(response, "0810", 0, success: true, 597029414300, "IM750164");
            AssertBaseResponseText(loadKeysResponseText, "0810", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task ShouldSendNackAndKeepWaiting_WhenResponseLrcIsInvalid()
        {
            const string commandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            AssertSentCommand(commandPayload);
            SendAck();
            _mockHandler.SimulateIncoming(BuildFrameWithInvalidLrc(responsePayload));

            Assert.False(task.IsCompleted);
            Assert.Equal(((char)0x15).ToString(), _mockHandler.WrittenData.Last());

            SendResponse(responsePayload);

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            AssertBasicResponse(response, "0810", 0, success: true, 597029414300, "IM750164");
            AssertBaseResponseText(loadKeysResponseText, "0810", 0);
            AssertFinalAckWritten(3);
        }
    }
}
