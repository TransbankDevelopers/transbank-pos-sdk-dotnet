using Transbank.Tests.Mocks;
using Transbank.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace Transbank.Tests
{
    public class PosServicesTests
    {
        private readonly MockSerialHandler _mock;
        private readonly PosService _service;
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;
        private const char ACK = (char)0x06;

        public PosServicesTests()
        {
            _mock = new MockSerialHandler();
            _service = new PosService(_mock);
        }

        [Fact]
        public void GetPorts_ShouldReturnFakePort()
        {
            var ports = _service.getPorts();

            Assert.Single(ports);
            Assert.Contains("FAKE_PORT", ports);
        }

        [Fact]
        public void OpenPort_And_ClosePort_ShouldToggle_IsPortOpen()
        {
            _service.OpenPort("FAKE_PORT");
            bool opened = _service.IsPortOpen;

            _service.ClosePort();
            bool closed = !_service.IsPortOpen;

            Assert.True(opened);
            Assert.True(closed);
        }

        [Fact]
        public async Task SendNormalCommand_ShouldReturnResponse_WhenValidMessageReceived()
        {
            string payload = "0800";
            string expected = $"{STX}{payload}{ETX}{(char)0x0B}";

            var task = _service.ProcessNormalCommand(payload);
            _mock.SimulateIncoming(expected);

            string response = await task;

            Assert.Contains(payload, response);
            Assert.Contains(((char)0x03).ToString(), response);
        }

        [Fact]
        public async Task SendNormalCommand_ShouldReturnAck_WhenShortResponseEnabled()
        {
            string payload = "TEST";
            string expected = $"{ACK}";

            var task = _service.ProcessNormalCommand(payload, shortResponse: true);
            _mock.SimulateIncoming(expected);
            string response = await task;

            Assert.Equal(PosService.ACK.ToString(), response);
        }

        [Fact]
        public async Task SendDetailsCommand_ShouldReturnList_WhenTwoEmptyAuthsDetected()
        {
            string payload = "SALE";
            string response1 = $"{STX}0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|{ETX}{(char)0x2E}";
            string response2 = $"{STX}0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|{ETX}{(char)0x2E}";
            string response3 = $"{STX}0261|00|597029414300|IT750050||||||||||||||||{ETX}{(char)0x62}";
            string response4 = $"{STX}0261|00|597029414300|IT750050||||||||||||||||{ETX}{(char)0x62}";

            var task = _service.ProcessDetailsCommand(payload, printOnPOS: false);

            _mock.SimulateIncoming(response1);
            _mock.SimulateIncoming(response2);
            _mock.SimulateIncoming(response3);
            _mock.SimulateIncoming(response4);

            List<string> response = await task;

            Assert.Equal(4, response.Count);
            Assert.Contains("0261", response[0]);
        }
    }
}
