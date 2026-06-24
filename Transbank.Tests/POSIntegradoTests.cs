using Transbank.Responses.CommonResponses;
using Transbank.Responses.IntegradoResponses;
using Transbank.Exceptions.IntegradoExceptions;
using Transbank.Services;
using Transbank.Tests.Helpers;
using Transbank.Tests.Mocks;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Transbank.Tests
{
    public class POSIntegradoTests
    {
        private readonly MockSerialHandler _mockHandler;
        private readonly PosService _service;
        private readonly POSIntegrado.POSIntegrado _pos;
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;
        private const char ACK = (char)0x06;

        public POSIntegradoTests()
        {
            _mockHandler = new MockSerialHandler();
            _service = new PosService(_mockHandler);
            _pos = new POSIntegrado.POSIntegrado(_mockHandler, _service);
        }

        [Fact]
        public void ListPorts_ShouldReturnFakePort()
        {
            var ports = _pos.ListPorts();
            Assert.Single(ports);
            Assert.Equal("FAKE_PORT", ports[0]);
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
        public async Task LoadKeys_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0810|00|597029414300|IT750050|{ETX}{(char)0x12}";
            var task = _pos.LoadKeys();
            _mockHandler.SimulateIncoming(expected);

            LoadKeysResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal("0810", response.FunctionCode);
        }

        [Fact]
        public async Task Sale_ShouldReturnValidResponse_WhenAmountIsValid()
        {
            string expected = $"{STX}0210|07|||ABC||10001||||||||||||||{ETX}{(char)0x77}";
            var task = _pos.Sale(10001, "ABC");
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(07, response.ResponseCode);
        }

        [Fact]
        public async Task Sale_ShouldThrowException_WhenAmountIsLessThan50()
        {
            await Assert.ThrowsAsync<TransbankSaleException>(async () =>
            {
                await _pos.Sale(30, "TICKET123");
            });
        }

        [Fact]
        public async Task MultiCommand_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0271|07|||abc123||12000|||||||||||||||597012345678|{ETX}{(char)0x60}";
            var task = _pos.MultiCodeSale(12000, "abc123", 597012345678, false, false);
            _mockHandler.SimulateIncoming(expected);

            MultiCodeSaleResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(07, response.ResponseCode);
            Assert.Equal(597012345678, response.CommerceProviderCode);
        }

        [Fact]
        public async Task LastSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0260|00|597029414300|IT750050|abc123|757752|12000|00|0|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||||{ETX}{(char)0x63}";
            var task = _pos.LastSale();
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(00, response.ResponseCode);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("MC", response.CardBrand);
        }

        [Fact]
        public async Task MultiCodeLastSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0281|00|597029414300|IT750050|abc123|757752|12000|00|0|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||||0|597012345678|{ETX}{(char)0x5F}";
            var task = _pos.MultiCodeLastSale(false);
            _mockHandler.SimulateIncoming(expected);

            MultiCodeLastSaleResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(00, response.ResponseCode);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("MC", response.CardBrand);
            Assert.Equal(597012345678, response.CommerceProviderCode);
        }

        [Fact]
        public async Task Totals_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0710|00|005|60000||{ETX}{(char)0x7A}";
            var task = _pos.Totals();
            _mockHandler.SimulateIncoming(expected);

            TotalsResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(00, response.ResponseCode);
            Assert.Equal(60000, response.TxTotal);
        }

        [Fact]
        public async Task Details_ShouldReturnValidResponseList()
        {
            string response1 = $"{STX}0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|{ETX}{(char)0x2E}";
            string response2 = $"{STX}0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|{ETX}{(char)0x2E}";
            string response3 = $"{STX}0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|{ETX}{(char)0x2E}";
            string response4 = $"{STX}0261|00|597029414300|IT750050||||||||||||||||{ETX}{(char)0x62}";
            string response5 = $"{STX}0261|00|597029414300|IT750050||||||||||||||||{ETX}{(char)0x62}";
            var task = _pos.Details(false);

            _mockHandler.SimulateIncoming(response1);
            _mockHandler.SimulateIncoming(response2);
            _mockHandler.SimulateIncoming(response3);
            _mockHandler.SimulateIncoming(response4);
            _mockHandler.SimulateIncoming(response5);

            List<DetailResponse> responses = await task;

            Assert.NotNull(responses);
            Assert.Equal(3, responses.Count);
        }
        [Fact]
        public async Task MultiCodeDetails_ShouldReturnValidResponseList()
        {
            string response1 = $"{STX}0291|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|0|597012345678|{ETX}{(char)0x12}";
            string response2 = $"{STX}0291|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|0|597012345678|{ETX}{(char)0x12}";
            string response3 = $"{STX}0291|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|000000|0000000000000000000|MC|22102025|114016||0|0|00|0|597012345678|{ETX}{(char)0x12}";
            string response4 = $"{STX}0291|00|597029414300|IT750050||||||||||||||||||{ETX}{(char)0x6D}";
            string response5 = $"{STX}0291|00|597029414300|IT750050||||||||||||||||||{ETX}{(char)0x6D}";
            var task = _pos.MultiCodeDetails(false);

            _mockHandler.SimulateIncoming(response1);
            _mockHandler.SimulateIncoming(response2);
            _mockHandler.SimulateIncoming(response3);
            _mockHandler.SimulateIncoming(response4);
            _mockHandler.SimulateIncoming(response5);

            List<MultiCodeDetailResponse> responses = await task;

            Assert.NotNull(responses);
            Assert.Equal(3, responses.Count);
        }

        [Fact]
        public async Task Close_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0510|00|597029414300|IT750050||{ETX}{(char)0x63}";
            var task = _pos.Close();
            _mockHandler.SimulateIncoming(expected);

            CloseResponse response = await task;

            Assert.NotNull(response);
            Assert.Equal(00, response.ResponseCode);
            Assert.Equal(597029414300, response.CommerceCode);
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
        public async Task SetNormalMode_ShouldReturnValidResponse()
        {
            string expected = $"{ACK}";
            var task = _pos.SetNormalMode();
            _mockHandler.SimulateIncoming(expected);

            bool response = await task;

            Assert.True(response);
        }

        [Fact]
        public async Task Sale_ShouldRaiseCleanIntermediateResponses_BeforeFinalResponse()
        {
            string finalResponsePayload = "0210|00|597029414300|IT750050|ABC123|925171|1200|00|0|3331|000072|DB|000000|0000000000000000331|P|16032026|120653||||";
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

            var task = _pos.Sale(1200, "ABC123", sendVoucher: true, sendStatus: true);

            foreach (string intermediatePayload in intermediatePayloads)
            {
                _mockHandler.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(intermediatePayload));
                Assert.False(task.IsCompleted);
            }

            _mockHandler.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(finalResponsePayload));

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
        }

        [Fact]
        public async Task Sale_ShouldKeepFlowAlive_WhenIntermediateResponsesContainParseErrors()
        {
            string finalResponsePayload = "0210|00|597029414300|IT750050|ABC123|925171|1200|00|0|3331|000072|DB|000000|0000000000000000331|P|16032026|120653||||";
            string[] intermediatePayloads = { "0900|84", "0900|", "0900|abc", "0900|82" };
            List<IntermediateResponse> responses = new();

            _pos.IntermediateResponseChange += (_, response) => responses.Add(response);

            var task = _pos.Sale(1200, "ABC123", sendVoucher: true, sendStatus: true);

            foreach (string intermediatePayload in intermediatePayloads)
            {
                _mockHandler.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(intermediatePayload));
                Assert.False(task.IsCompleted);
            }

            _mockHandler.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(finalResponsePayload));

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
