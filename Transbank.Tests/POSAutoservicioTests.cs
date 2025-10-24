using Transbank.Responses.CommonResponses;
using Transbank.Services;
using Transbank.Tests.Mocks;
using Transbank.Responses.AutoservicioResponse;


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
            Assert.Contains("FAKE_PORT", portsList);
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
            Assert.Contains("0810", response.FunctionCode);
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

            Assert.Contains("0210", response.FunctionCode);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0271|78{ETX}{(char)0x76}";
            var task = _pos.MultiCodeSale(1000, "ABC123");
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Contains("0271", response.FunctionCode);
        }

        [Fact]
        public async Task LastSale_ShouldReturnValidResponse()
        {
            string expected = $"{STX}0260|11|597029414300|IM750015{ETX}{(char)0x79}";
            var task = _pos.LastSale();
            _mockHandler.SimulateIncoming(expected);

            SaleResponse response = await task;

            Assert.Contains("0260", response.FunctionCode);
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
    }
}
