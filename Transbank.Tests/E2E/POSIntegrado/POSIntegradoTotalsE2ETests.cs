using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests.E2E.POSIntegrado
{
    public class POSIntegradoTotalsE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async System.Threading.Tasks.Task Totals_ShouldParseApprovedResponseWithSales()
        {
            const string expectedCommandPayload = "0700|";

            var task = _pos.Totals();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(TotalsWithSalesResponsePayload);

            TotalsResponse response = await task;
            string totalsResponseText = response.ToString();

            Assert.Equal("0710", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal("Aprobado", response.ResponseMessage);
            Assert.True(response.Success);
            Assert.Equal(2, response.TxCount);
            Assert.Equal(15000, response.TxTotal);
            AssertBaseResponseText(totalsResponseText, "0710", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async System.Threading.Tasks.Task Totals_ShouldParseApprovedResponseWithoutSales()
        {
            const string expectedCommandPayload = "0700|";

            var task = _pos.Totals();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(TotalsWithoutSalesResponsePayload);

            TotalsResponse response = await task;
            string totalsResponseText = response.ToString();

            Assert.Equal("0710", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal("Aprobado", response.ResponseMessage);
            Assert.True(response.Success);
            Assert.Equal(0, response.TxCount);
            Assert.Equal(0, response.TxTotal);
            AssertBaseResponseText(totalsResponseText, "0710", 0);
            AssertFinalAckWritten(2);
        }
    }
}
