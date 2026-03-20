using System;
using System.Linq;
using System.Threading.Tasks;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Responses.CommonResponses;
using Transbank.Services;
using Transbank.Tests.Mocks;
using Xunit;

namespace Transbank.Tests.E2E
{
    public abstract class POSAutoservicioE2ETestBase
    {
        protected readonly MockSerialHandler _mockHandler;
        protected readonly POSAutoservicio.POSAutoservicio _pos;
        protected const char STX = (char)0x02;
        protected const char ETX = (char)0x03;
        protected const char ACK = (char)0x06;
        protected const string SaleDebitWithVoucherResponsePayload =
            "0210|00|597029414300|IM750164|123456|547545|1000|3331|55|DB|00-00-00|331|P |18032026|123230|" +
            POSAutoservicioVoucherFixtures.SaleDebitVoucher;
        protected const string SaleCreditWithVoucherResponsePayload =
            "0210|00|597029414300|IM750164|123456|316557|10000|6590|57|CR|||VI|18032026|123429|" +
            POSAutoservicioVoucherFixtures.SaleCreditVoucher +
            "|03|03|3334|CUOTAS SIN INTERES";
        protected const string MultiCodeSaleDebitWithVoucherResponsePayload =
            "0271|00|597029414303|IM750164|123456|475618|1000|3331|62|DB|00-00-00|331|P |18032026|171040|597012345678|" +
            POSAutoservicioVoucherFixtures.MultiCodeSaleDebitVoucher;
        protected const string MultiCodeSaleCreditWithVoucherResponsePayload =
            "0271|00|597029414303|IM750164|123456|194937|10000|6590|64|CR|||VI|18032026|171153|597012345678|" +
            POSAutoservicioVoucherFixtures.MultiCodeSaleCreditVoucher +
            "|03|03|3334|CUOTAS SIN INTERES";
        protected const string LastSaleDebitWithVoucherResponsePayload =
            "0260|00|597029414303|IM750164|123456|912108|1000|3331|68|DB|00-00-00|331|P |19032026|102438|" +
            POSAutoservicioVoucherFixtures.LastSaleDebitVoucher;
        protected const string LastSaleCreditWithVoucherResponsePayload =
            "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006|" +
            POSAutoservicioVoucherFixtures.LastSaleCreditVoucher +
            "|03|03|3334|CUOTAS SIN INTERES";
        protected const string CloseWithDataVoucherResponsePayload =
            "0510|00|597029414300|IM750164|" +
            POSAutoservicioVoucherFixtures.CloseWithDataVoucher;
        protected const string CloseWithoutDataVoucherResponsePayload =
            "0510|00|597029414300|IM750164|" +
            POSAutoservicioVoucherFixtures.CloseWithoutDataVoucher;

        protected POSAutoservicioE2ETestBase()
        {
            _mockHandler = new MockSerialHandler();
            var service = new PosService(_mockHandler, PosService.Model.AUTOSERVICIO);
            _pos = new POSAutoservicio.POSAutoservicio(_mockHandler, service);
        }

        protected static string BuildCommandFrame(string payload)
        {
            char lrc = ETX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            return $"{STX}{payload}{ETX}{lrc}";
        }

        protected static string BuildResponseFrame(string payload)
        {
            char lrc = STX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            lrc ^= ETX;
            return $"{STX}{payload}{ETX}{lrc}";
        }

        protected static string BuildFrameWithInvalidLrc(string payload)
        {
            return $"{STX}{payload}{ETX}{(char)0x00}";
        }

        protected void AssertSentCommand(string payload)
        {
            Assert.Equal(BuildCommandFrame(payload), _mockHandler.WrittenData.Single());
        }

        protected void SendAck()
        {
            _mockHandler.SimulateIncoming(ACK.ToString());
        }

        protected void SendResponse(string payload)
        {
            _mockHandler.SimulateIncoming(BuildResponseFrame(payload));
        }

        protected void SendFragmentedResponseAndAssertPending(Task task, string payload)
        {
            string fullResponseFrame = BuildResponseFrame(payload);
            int splitIndex = fullResponseFrame.Length / 2;
            _mockHandler.SimulateIncoming(fullResponseFrame[..splitIndex]);
            Assert.False(task.IsCompleted);
            _mockHandler.SimulateIncoming(fullResponseFrame[splitIndex..]);
        }

        protected void AssertFinalAckWritten(int expectedWriteCount)
        {
            Assert.Equal(expectedWriteCount, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        protected static void AssertBasicResponse(BasicResponse response, string functionCode, int responseCode, bool success, long commerceCode, string? terminalId)
        {
            Assert.NotNull(response);
            Assert.Equal(functionCode, response.FunctionCode);
            Assert.Equal(responseCode, response.ResponseCode);
            Assert.Equal(success, response.Success);

            if (commerceCode != 0 && response is LoadKeysResponse loadKeysResponse)
            {
                Assert.Equal(commerceCode, loadKeysResponse.CommerceCode);
            }

            if (terminalId != null && response is LoadKeysResponse loadKeysTerminalResponse)
            {
                Assert.Equal(terminalId, loadKeysTerminalResponse.TerminalId);
            }
        }

        protected static void AssertBaseResponseText(string responseText, string functionCode, int responseCode)
        {
            Assert.Contains($"Function: {functionCode}", responseText);
            Assert.Contains($"Response code:{responseCode}", responseText);
        }

        protected static void AssertSaleFields(SaleResponse response, string ticket, string authorizationCode, int amount, int last4Digits, int operationNumber, string cardType, string accountNumber, string cardBrand, DateTime realDate)
        {
            Assert.Equal(ticket, response.Ticket);
            Assert.Equal(authorizationCode, response.AuthorizationCode);
            Assert.Equal(amount, response.Amount);
            Assert.Equal(last4Digits, response.Last4Digits);
            Assert.Equal(operationNumber, response.OperationNumber);
            Assert.Equal(cardType, response.CardType);
            Assert.Equal(accountNumber, response.AccountNumber);
            Assert.Equal(cardBrand, response.CardBrand);
            Assert.Equal(realDate, response.RealDate);
        }

        protected static void AssertInstallments(SaleResponse response, int installmentsType, int installmentsNumber, int installmentsAmount, string installmentsTypeDescription)
        {
            Assert.Equal(installmentsType, response.InstallmentsType);
            Assert.Equal(installmentsNumber, response.InstallmentsNumber);
            Assert.Equal(installmentsAmount, response.InstallmentsAmount);
            Assert.Equal(installmentsTypeDescription, response.InstallmentsTypeDescription);
        }

        protected static void AssertInstallments(MultiCodeSaleResponse response, int installmentsType, int installmentsNumber, int installmentsAmount, string installmentsTypeDescription)
        {
            Assert.Equal(installmentsType, response.InstallmentsType);
            Assert.Equal(installmentsNumber, response.InstallmentsNumber);
            Assert.Equal(installmentsAmount, response.InstallmentsAmount);
            Assert.Equal(installmentsTypeDescription, response.InstallmentsTypeDescription);
        }

        protected static void AssertEmptyPrintingField(System.Collections.Generic.IReadOnlyList<string> printingField)
        {
            Assert.Empty(printingField);
        }

        protected static void AssertVoucherLinesHaveFixedWidth(System.Collections.Generic.IReadOnlyList<string> printingField)
        {
            Assert.NotEmpty(printingField);
            if (printingField.Count > 1)
            {
                Assert.All(printingField, line => Assert.Equal(40, line.Length));
            }
        }
    }
}
