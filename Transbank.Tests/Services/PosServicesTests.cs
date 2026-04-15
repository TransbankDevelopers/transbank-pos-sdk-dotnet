using Transbank.Tests.Mocks;
using Transbank.Services;
using Transbank.Tests.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Equal("FAKE_PORT", ports[0]);
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
            string expected = TestFrameBuilder.BuildCommandFrame(payload);

            var task = _service.ProcessNormalCommand(payload);
            _mock.SimulateIncoming(expected);

            string response = await task;

            Assert.Equal(payload, response);
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

            Assert.Equal(2, response.Count);
            Assert.Equal("0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|", response[0]);
        }

        [Fact]
        public async Task ProcessNormalCommand_ShouldPublishSanitizedIntermediatePayload_AndKeepWaitingForFinalResponse()
        {
            const string commandPayload = "0200|000001200|123asd||1|1|";
            const string finalResponsePayload = "0210|00|597029414300|IT750050|123asd|925171|1200|00|0|3331|72|DB|000000|0000000000000000331|P|16032026|120653||||";
            string[] intermediatePayloads = { "0900|84", "0900|83", "0900|81", "0900|82" };
            List<string> receivedIntermediates = new();

            _service.IntermediateResponseReceived += (_, response) => receivedIntermediates.Add(response);

            var task = _service.ProcessNormalCommand(commandPayload);

            foreach (string intermediatePayload in intermediatePayloads)
            {
                _mock.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(intermediatePayload));
                Assert.False(task.IsCompleted);
            }

            _mock.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(finalResponsePayload));

            string response = await task;

            Assert.Equal(intermediatePayloads, receivedIntermediates);
            Assert.All(receivedIntermediates, message =>
            {
                Assert.Equal(-1, message.IndexOf(STX));
                Assert.Equal(-1, message.IndexOf(ETX));
            });
            Assert.Equal(finalResponsePayload, response);
            Assert.Equal(1, _mock.WrittenData.Count(data => data == ACK.ToString()));
        }
    }
}
