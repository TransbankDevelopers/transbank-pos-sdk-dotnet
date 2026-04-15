using System;
using System.Threading.Tasks;
using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSIntegradoMultiCodeSaleE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedCreditResponseWithVoucher()
        {
            const string expectedCommandPayload = "0270|12000|ABC123||1|0|597029414303|";

            var task = _pos.MultiCodeSale(12000, "ABC123", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendResponse(MultiCodeSaleCreditWithVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal("0271", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal("ABC123", response.Ticket);
            Assert.Equal("794160", response.AuthorizationCode);
            Assert.Equal(12000, response.Amount);
            Assert.Equal(3, response.InstallmentsNumber);
            Assert.Equal(4000, response.InstallmentsAmount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(141, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("3000000000000000000", response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 4, 6, 23, 41, 9), response.RealDate);
            Assert.Equal(0, response.EmployeeId);
            Assert.Equal(0, response.Tip);
            Assert.Equal(POSIntegradoVoucherFixtures.MultiCodeSaleCreditVoucher, response.RawVoucher);
            Assert.Equal(46, response.PrintingField.Count);
            Assert.Equal("               TRANSBANK                ", response.PrintingField[0]);
            Assert.Equal("     VENTA CON PIN - COPIA COMERCIO     ", response.PrintingField[1]);
            Assert.Equal("                                        ", response.PrintingField[22]);
            Assert.Equal("               TRANSBANK                ", response.PrintingField[23]);
            Assert.Equal("     VENTA CON PIN - COPIA CLIENTE      ", response.PrintingField[24]);
            Assert.Equal("OPERACION: 000141   AUTORIZACION: 794160", response.PrintingField[39]);
            Assert.Equal("                                        ", response.PrintingField[45]);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            Assert.Equal(0, response.Change);
            Assert.Equal(597029414303, response.CommerceProviderCode);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }
    }
}
