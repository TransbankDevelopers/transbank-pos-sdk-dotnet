using System.Linq;
using System.Threading.Tasks;
using Transbank.Tests.Helpers;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoCoreE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task Poll_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0100";

            var task = _pos.Poll();

            Assert.Equal(TestFrameBuilder.BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            SendAck();

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

    }
}
