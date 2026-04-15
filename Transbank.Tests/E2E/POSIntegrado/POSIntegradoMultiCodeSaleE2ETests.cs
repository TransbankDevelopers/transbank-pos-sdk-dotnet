using System;
using System.Threading.Tasks;
using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests.E2E.POSIntegrado
{
    public class POSIntegradoMultiCodeSaleE2ETests : POSIntegradoE2ETestBase
    {
        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedDebitResponseWithVoucher()
        {
            const string expectedCommandPayload = "0270|8000|ABC123||1|0|597029414303|";

            var task = _pos.MultiCodeSale(8000, "ABC123", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(MultiCodeSaleDebitWithVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal("0271", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal("ABC123", response.Ticket);
            Assert.Equal("708410", response.AuthorizationCode);
            Assert.Equal(8000, response.Amount);
            Assert.Equal(0, response.InstallmentsNumber);
            Assert.Equal(0, response.InstallmentsAmount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(143, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("********331", response.AccountNumber);
            Assert.Equal("DB", response.CardBrand);
            Assert.Equal(new DateTime(2026, 4, 7, 8, 50, 34), response.RealDate);
            Assert.Equal(0, response.EmployeeId);
            Assert.Equal(0, response.Tip);
            Assert.Equal(POSIntegradoVoucherFixtures.MultiCodeSaleDebitVoucher, response.RawVoucher);
            Assert.Equal(42, response.PrintingField.Count);
            Assert.Equal("               TRANSBANK                ", response.PrintingField[0]);
            Assert.Equal("         VENTA - COPIA COMERCIO         ", response.PrintingField[1]);
            Assert.Equal("OPERACION: 000143   AUTORIZACION: 708410", response.PrintingField[14]);
            Assert.Equal("               TRANSBANK                ", response.PrintingField[21]);
            Assert.Equal("         VENTA - COPIA CLIENTE          ", response.PrintingField[22]);
            Assert.Equal("OPERACION: 000143   AUTORIZACION: 708410", response.PrintingField[35]);
            Assert.Equal("                                        ", response.PrintingField[41]);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            Assert.Equal(0, response.Change);
            Assert.Equal(597029414303, response.CommerceProviderCode);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedDebitResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0270|7000|ABC123||0|0|597029414303|";

            var task = _pos.MultiCodeSale(7000, "ABC123", 597029414303);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(MultiCodeSaleDebitWithoutVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal("0271", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal("ABC123", response.Ticket);
            Assert.Equal("388892", response.AuthorizationCode);
            Assert.Equal(7000, response.Amount);
            Assert.Equal(0, response.InstallmentsNumber);
            Assert.Equal(0, response.InstallmentsAmount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(144, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("********331", response.AccountNumber);
            Assert.Equal("DB", response.CardBrand);
            Assert.Equal(new DateTime(2026, 4, 7, 8, 56, 28), response.RealDate);
            Assert.Equal(0, response.EmployeeId);
            Assert.Equal(0, response.Tip);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Empty(response.PrintingField);
            Assert.Equal(0, response.Change);
            Assert.Equal(597029414303, response.CommerceProviderCode);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task MultiCodeSale_ShouldParseCancelledResponse()
        {
            const string expectedCommandPayload = "0270|90000|ABC123||1|0|597029414303|";

            var task = _pos.MultiCodeSale(90000, "ABC123", 597029414303, sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(MultiCodeSaleCancelledResponsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal("0271", response.FunctionCode);
            Assert.Equal(7, response.ResponseCode);
            Assert.Equal("Transacción cancelada desde el POS", response.ResponseMessage);
            Assert.False(response.Success);
            Assert.Equal(0, response.CommerceCode);
            Assert.Equal(string.Empty, response.TerminalId);
            Assert.Equal("ABC123", response.Ticket);
            Assert.Equal(string.Empty, response.AuthorizationCode);
            Assert.Equal(90000, response.Amount);
            Assert.Equal(0, response.InstallmentsNumber);
            Assert.Equal(0, response.InstallmentsAmount);
            Assert.Equal(0, response.Last4Digits);
            Assert.Equal(0, response.OperationNumber);
            Assert.Equal(string.Empty, response.CardType);
            Assert.Null(response.AccountingDate);
            Assert.Equal(string.Empty, response.AccountNumber);
            Assert.Equal(string.Empty, response.CardBrand);
            Assert.Null(response.RealDate);
            Assert.Equal(0, response.EmployeeId);
            Assert.Equal(0, response.Tip);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Empty(response.PrintingField);
            Assert.Equal(0, response.Change);
            Assert.Equal(597029414303, response.CommerceProviderCode);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 7);
            AssertFinalAckWritten(2);
        }

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

        [Fact]
        public async Task MultiCodeSale_ShouldParseApprovedCreditResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0270|9000|ABC123||0|0|597029414303|";

            var task = _pos.MultiCodeSale(9000, "ABC123", 597029414303);

            AssertSentCommand(expectedCommandPayload);
            SendResponse(MultiCodeSaleCreditWithoutVoucherResponsePayload);

            MultiCodeSaleResponse response = await task;
            string multiCodeSaleResponseText = response.ToString();

            Assert.Equal("0271", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IT750050", response.TerminalId);
            Assert.Equal("ABC123", response.Ticket);
            Assert.Equal("162529", response.AuthorizationCode);
            Assert.Equal(9000, response.Amount);
            Assert.Equal(3, response.InstallmentsNumber);
            Assert.Equal(3000, response.InstallmentsAmount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(142, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("3000000000000000000", response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 4, 7, 8, 49, 18), response.RealDate);
            Assert.Equal(0, response.EmployeeId);
            Assert.Equal(0, response.Tip);
            Assert.True(string.IsNullOrWhiteSpace(response.RawVoucher));
            Assert.Empty(response.PrintingField);
            Assert.Equal(0, response.Change);
            Assert.Equal(597029414303, response.CommerceProviderCode);
            AssertBaseResponseText(multiCodeSaleResponseText, "0271", 0);
            AssertFinalAckWritten(2);
        }
    }
}
