using Transbank.Services;
using Transbank.Tests.Mocks;

namespace Transbank.Tests.E2E
{
    public abstract class POSIntegradoE2ETestBase
    {
        protected readonly MockSerialHandler _mockHandler;
        protected readonly PosService _service;
        protected readonly POSIntegrado.POSIntegrado _pos;
        protected const char ACK = (char)0x06;

        protected POSIntegradoE2ETestBase()
        {
            _mockHandler = new MockSerialHandler();
            _service = new PosService(_mockHandler);
            _pos = new POSIntegrado.POSIntegrado(_mockHandler, _service);
        }

        protected void SendAck()
        {
            _mockHandler.SimulateIncoming(ACK.ToString());
        }
    }
}
