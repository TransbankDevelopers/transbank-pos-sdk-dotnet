using Transbank.Responses.CommonResponses;
using Transbank.Services;
using Transbank.Tests.Mocks;
using Transbank.Tests.Helpers;
using Transbank.Responses.AutoservicioResponse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Transbank.Tests
{
    public class POSAutoservicioTests
    {
        private readonly MockSerialHandler _mockHandler;
        private readonly PosService _serviceAutoservicio;
        private readonly POSAutoservicio.POSAutoservicio _pos;
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;
        private const char ACK = (char)0x06;

        public POSAutoservicioTests()
        {
            _mockHandler = new MockSerialHandler();
            _serviceAutoservicio = new PosService(_mockHandler, PosService.Model.AUTOSERVICIO);
            _pos = new POSAutoservicio.POSAutoservicio(_mockHandler, _serviceAutoservicio);
        }

        [Fact]
        public void ListPorts_ShouldReturnFakePort()
        {
            var portsList = _pos.ListPorts();
            Assert.Single(portsList);
            Assert.Equal("FAKE_PORT", portsList[0]);
        }

        [Fact]
        public void OpenPort_And_ClosePort_ShouldToggle_IsPortOpen()
        {
            _pos.OpenPort("FAKE_PORT");
            Assert.True(_pos.IsPortOpen);

            _pos.ClosePort();
            Assert.False(_pos.IsPortOpen);
        }

        [Fact]
        public async Task Poll_ShouldReturnValidResponse()
        {
            string expected = $"{ACK}";
            var task = _pos.Poll();
            _mockHandler.SimulateIncoming(expected);

            bool response = await task;

            Assert.True(response);
        }

        [Fact]
        public async Task LoadKeys_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0810|00|597029414300|IM750015{ETX}{(char)0x74}";
            var task = _pos.LoadKeys();
            _mockHandler.SimulateIncoming(expected);

            LoadKeysResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal("0810", response.FunctionCode);
        }

        [Fact]
        public async Task Initialization_ShouldReturnValidResponse()
        {
            string expected = $"{ACK}";
            var task = _pos.Initialization();
            _mockHandler.SimulateIncoming(expected);

            bool response = await task;

            Assert.True(response);
        }

        [Fact]
        public async Task InitializationResponse_ShouldReturnValidResponse()
        {
            string expected = $"{STX}1080|90|06102025|151913{ETX}{(char)0x71}";
            var task = _pos.InitializationResponse();
            _mockHandler.SimulateIncoming(expected);

            InitializationResponse response = await task;

            Assert.Equal(90, response.ResponseCode);
        }

        [Fact]
        public async Task Sale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0210|07{ETX}{(char)0x79}";
            var task = _pos.Sale(1000, "ABC123");
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Equal("0210", response.FunctionCode);
        }

        [Fact]
        public async Task Canceled_Sale_ShouldSetValidResponseCode()
        {
            string expected = $"{STX}0210|07{ETX}{(char)0x79}";
            var task = _pos.Sale(1000, "ABC123");
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Equal(07, response.ResponseCode);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0271|78{ETX}{(char)0x76}";
            var task = _pos.MultiCodeSale(1000, "ABC123");
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Equal("0271", response.FunctionCode);
        }

        [Fact]
        public async Task LastSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0260|11|597029414300|IM750015{ETX}{(char)0x79}";
            var task = _pos.LastSale();
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Equal("0260", response.FunctionCode);
        }

        [Fact]
        public async Task PrintLastSaleResponse_ShouldResultOk()
        {
            string expected = $"{STX}0260|00|597029414300|IM750015|abc123|414170|12000|6590|62|CR|||VI|28102025|174756{ETX}{(char)0x69}";
            var task = _pos.LastSale();
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750015", response.TerminalId);
        }

        [Fact]
        public async Task Close_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0510|00|597029414300|IT750050||{ETX}{(char)0x61}";
            var task = _pos.Close(false);
            _mockHandler.SimulateIncoming(expected);

            CloseResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(00, response.ResponseCode);
            Assert.Equal(597029414300, response.CommerceCode);
        }

        [Fact]
        public async Task Sale_ShouldRaiseCleanIntermediateResponses_BeforeFinalResponse()
        {
            const string finalResponsePayload = "0210|00|597029414300|IM750015|123asd|925171|1200|3331|72|DB|10032026|331|P |16032026|120653";
            string[] intermediatePayloads = { "0900|84", "0900|83", "0900|81", "0900|82" };
            int[] expectedCodes = { 84, 83, 81, 82 };
            string[] expectedMessages =
            {
                "Opere tarjeta",
                "Selección menú crédito/redcompra",
                "Solicitando ingreso de clave",
                "Enviando transacción al host"
            };
            List<IntermediateResponse> responses = new();

            _pos.IntermediateResponseChange += (_, response) => responses.Add(response);

            var task = _pos.Sale(1200, "123asd", sendVoucher: true, sendStatus: true);

            foreach (string intermediatePayload in intermediatePayloads)
            {
                _mockHandler.SimulateIncoming(TestFrameBuilder.BuildResponseFrame(intermediatePayload));
                Assert.False(task.IsCompleted);
            }

            _mockHandler.SimulateIncoming(TestFrameBuilder.BuildResponseFrame(finalResponsePayload));

            SaleResponse saleResponse = await task;

            Assert.Equal(4, responses.Count);
            for (int i = 0; i < responses.Count; i++)
            {
                Assert.Equal("0900", responses[i].FunctionCode);
                Assert.Equal(expectedCodes[i], responses[i].ResponseCode);
                Assert.Equal(expectedMessages[i], responses[i].ResponseMessage);
                Assert.Equal(-1, responses[i].FunctionCode.IndexOf(STX));
                Assert.Equal(-1, responses[i].FunctionCode.IndexOf(ETX));
            }

            Assert.Equal(0, saleResponse.ResponseCode);
            Assert.Equal(1, _mockHandler.WrittenData.Count(data => data == ACK.ToString()));
            Assert.Equal(TestFrameBuilder.BuildCommandFrame("0200|1200|123asd|1|1"), _mockHandler.WrittenData[0]);
        }

        [Fact]
        public async Task Sale_ShouldKeepFlowAlive_WhenIntermediateResponsesContainParseErrors()
        {
            const string finalResponsePayload = "0210|00|597029414300|IM750015|123asd|925171|1200|3331|72|DB|10032026|331|P |16032026|120653";
            string[] intermediatePayloads = { "0900|84", "0900|", "0900|abc", "0900|82" };
            List<IntermediateResponse> responses = new();

            _pos.IntermediateResponseChange += (_, response) => responses.Add(response);

            var task = _pos.Sale(1200, "123asd", sendVoucher: true, sendStatus: true);

            foreach (string intermediatePayload in intermediatePayloads)
            {
                _mockHandler.SimulateIncoming(TestFrameBuilder.BuildResponseFrame(intermediatePayload));
                Assert.False(task.IsCompleted);
            }

            _mockHandler.SimulateIncoming(TestFrameBuilder.BuildResponseFrame(finalResponsePayload));

            SaleResponse saleResponse = await task;

            Assert.Equal(4, responses.Count);
            Assert.True(responses[0].Success);
            Assert.False(responses[1].HasValidParse);
            Assert.False(responses[1].Success);
            Assert.False(responses[2].HasValidParse);
            Assert.False(responses[2].Success);
            Assert.True(responses[3].Success);
            Assert.Equal(0, saleResponse.ResponseCode);
        }
    }
}
