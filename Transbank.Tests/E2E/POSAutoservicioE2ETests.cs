using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.CommonResponses;
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
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            Assert.Equal(BuildFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildFrame(responsePayload));

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

            Assert.Equal(BuildFrame(commandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildFrameWithInvalidLrc(responsePayload));

            Assert.False(task.IsCompleted);
            Assert.Equal(((char)0x15).ToString(), _mockHandler.WrittenData.Last());

            _mockHandler.SimulateIncoming(BuildFrame(responsePayload));

            LoadKeysResponse response = await task;

            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal(3, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        private static string BuildFrame(string payload)
        {
            char lrc = ETX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            return $"{STX}{payload}{ETX}{lrc}";
        }

        private static string BuildFrameWithInvalidLrc(string payload)
        {
            return $"{STX}{payload}{ETX}{(char)0x00}";
        }
    }
}
