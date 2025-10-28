using Transbank.Responses.CommonResponses;
using Transbank.Responses.IntegradoResponses;
using Transbank.Exceptions.IntegradoExceptions;
using Transbank.Services;
using Transbank.Tests.Mocks;
using System.Threading.Tasks;
using System.Collections.Generic;
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
            Assert.Contains("FAKE_PORT", ports);
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
            Assert.Contains("0810", response.FunctionCode);
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
            Assert.Equal(5, responses.Count);
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
            Assert.Equal(5, responses.Count);
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
    }
}
