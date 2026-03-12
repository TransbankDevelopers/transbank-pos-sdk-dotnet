using System.Linq;
using System.Threading.Tasks;
using System;
using Transbank.Responses.CommonResponses;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Services;
using Transbank.Tests.Mocks;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSAutoservicioE2ETests
    {
        private readonly MockSerialHandler _mockHandler;
        private readonly POSAutoservicio.POSAutoservicio _pos;
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;
        private const char ACK = (char)0x06;

        public POSAutoservicioE2ETests()
        {
            _mockHandler = new MockSerialHandler();
            var service = new PosService(_mockHandler, PosService.Model.AUTOSERVICIO);
            _pos = new POSAutoservicio.POSAutoservicio(_mockHandler, service);
        }

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

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            InitializationResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal("1080", response.FunctionCode);
            Assert.Equal(90, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(new DateTime(2026, 2, 3, 11, 15, 43), response.RealDate);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LoadKeysResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task ShouldSendNackAndKeepWaiting_WhenResponseLrcIsInvalid()
        {
            const string commandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            Assert.Equal(BuildCommandFrame(commandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildFrameWithInvalidLrc(responsePayload));

            Assert.False(task.IsCompleted);
            Assert.Equal(((char)0x15).ToString(), _mockHandler.WrittenData.Last());

            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LoadKeysResponse response = await task;

            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal(3, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        private static string BuildCommandFrame(string payload)
        {
            char lrc = ETX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            return $"{STX}{payload}{ETX}{lrc}";
        }

        private static string BuildResponseFrame(string payload)
        {
            char lrc = STX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            lrc ^= ETX;
            return $"{STX}{payload}{ETX}{lrc}";
        }

        private static string BuildFrameWithInvalidLrc(string payload)
        {
            return $"{STX}{payload}{ETX}{(char)0x00}";
        }
    }
}
