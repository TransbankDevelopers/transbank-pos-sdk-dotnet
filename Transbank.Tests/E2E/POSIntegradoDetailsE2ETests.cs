using System;
using System.Collections.Generic;
using System.Linq;
using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoDetailsE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async System.Threading.Tasks.Task Details_ShouldParseSales_WhenPrintOnPOSIsDisabled()
        {
            const string expectedCommandPayload = "0260|1|";

            var task = _pos.Details(false);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(DetailsDebitSaleOneResponsePayload);
            SendResponse(DetailsDebitSaleTwoResponsePayload);
            SendResponse("0261|00|597029414300|IT750050||||||||||||||||");
            SendResponse("0261|00|597029414300|IT750050||||||||||||||||");

            List<DetailResponse> responses = await task;

            Assert.Equal(5, _mockHandler.WrittenData.Count);
            Assert.Equal(2, responses.Count(response => response.AuthorizationCode == "708410" || response.AuthorizationCode == "388892"));

            DetailResponse firstResponse = responses[0];
            Assert.Equal("0261", firstResponse.FunctionCode);
            Assert.Equal(0, firstResponse.ResponseCode);
            Assert.True(firstResponse.Success);
            Assert.Equal(597029414300, firstResponse.CommerceCode);
            Assert.Equal("IT750050", firstResponse.TerminalId);
            Assert.Equal("ABC123", firstResponse.Ticket);
            Assert.Equal("708410", firstResponse.AuthorizationCode);
            Assert.Equal(8000, firstResponse.Amount);
            Assert.Equal(3331, firstResponse.Last4Digits);
            Assert.Equal(143, firstResponse.OperationNumber);
            Assert.Equal("DB", firstResponse.CardType);
            Assert.Equal(DateTime.MinValue, firstResponse.AccountingDate);
            Assert.Equal("********331", firstResponse.AccountNumber);
            Assert.Equal("DB", firstResponse.CardBrand);
            Assert.Equal(new DateTime(2026, 4, 7, 8, 50, 34), firstResponse.RealDate);
            Assert.Equal(0, firstResponse.EmployeeId);
            Assert.Equal(0, firstResponse.Tip);
            Assert.Equal(0, firstResponse.InstallmentsAmount);
            Assert.Equal(0, firstResponse.InstallmentsNumber);

            DetailResponse secondResponse = responses[1];
            Assert.Equal("388892", secondResponse.AuthorizationCode);
            Assert.Equal(7000, secondResponse.Amount);
            Assert.Equal(144, secondResponse.OperationNumber);
            Assert.Equal(new DateTime(2026, 4, 7, 8, 56, 28), secondResponse.RealDate);
            Assert.Equal(0, secondResponse.InstallmentsAmount);
            Assert.Equal(0, secondResponse.InstallmentsNumber);

            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async System.Threading.Tasks.Task Details_ShouldReturnEmptyList_WhenPrintOnPOSIsEnabledAndAckIsReceived()
        {
            const string expectedCommandPayload = "0260|0|";

            var task = _pos.Details();

            AssertSentCommand(expectedCommandPayload);
            SendAck();

            List<DetailResponse> responses = await task;

            Assert.Empty(responses);
            Assert.Single(_mockHandler.WrittenData);
        }

        [Fact]
        public async System.Threading.Tasks.Task Details_ShouldReturnEmptyList_WhenThereAreNoSalesAndPrintOnPOSIsDisabled()
        {
            const string expectedCommandPayload = "0260|1|";

            var task = _pos.Details(false);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(DetailsNoSalesResponsePayload);
            SendResponse(DetailsNoSalesResponsePayload);

            List<DetailResponse> responses = await task;

            Assert.Empty(responses);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }
    }
}
