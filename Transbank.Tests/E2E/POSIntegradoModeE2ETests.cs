using System.Linq;
using System.Threading.Tasks;
using Transbank.Tests.Helpers;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoModeE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task SetNormalMode_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0300";

            var task = _pos.SetNormalMode();

            Assert.Equal(TestFrameBuilder.BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            SendAck();

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }
    }
}
