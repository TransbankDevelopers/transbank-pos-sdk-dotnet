using System.Collections.Generic;
using System.Linq;
using Transbank.Responses.CommonResponses;
using Transbank.Services;
using Transbank.Tests.Helpers;
using Transbank.Tests.Mocks;
using Xunit;

namespace Transbank.Tests.E2E
{
    public abstract class POSIntegradoE2ETestBase
    {
        protected readonly MockSerialHandler _mockHandler;
        protected readonly PosService _service;
        protected readonly POSIntegrado.POSIntegrado _pos;
        protected const char ACK = (char)0x06;
        protected static readonly string MultiCodeSaleCreditWithVoucherResponsePayload =
            "0271|00|597029414300|IT750050|ABC123|794160|12000|03|4000|6590|000141|CR|003000|3000000000000000000|VI|06042026|234109|||" +
            POSIntegradoVoucherFixtures.MultiCodeSaleCreditVoucher +
            "|0|597029414303";
        protected static readonly string MultiCodeSaleDebitWithVoucherResponsePayload =
            "0271|00|597029414300|IT750050|ABC123|708410|8000|00||3331|000143|DB|000000|  ********331      |DB|07042026|085034|||" +
            POSIntegradoVoucherFixtures.MultiCodeSaleDebitVoucher +
            "|0|597029414303";
        protected const string MultiCodeSaleDebitWithoutVoucherResponsePayload =
            "0271|00|597029414300|IT750050|ABC123|388892|7000|00||3331|000144|DB|000000|  ********331      |DB|07042026|085628||||0|597029414303|";
        protected const string MultiCodeSaleCreditWithoutVoucherResponsePayload =
            "0271|00|597029414300|IT750050|ABC123|162529|9000|03|3000|6590|000142|CR|003000|3000000000000000000|VI|07042026|084918||||0|597029414303|";
        protected const string MultiCodeSaleCancelledResponsePayload =
            "0271|07|||ABC123||90000|||||||||||||||597029414303|";
        protected const string RefundDebitDeniedResponsePayload =
            "1210|21|597029414300|IT750050||143||";
        protected const string RefundCreditApprovedResponsePayload =
            "1210|00|597029414300|IT750050|162529|000142||";
        protected const string DetailsDebitSaleOneResponsePayload =
            "0261|00|597029414300|IT750050|ABC123|708410|8000|3331|000143|DB|000000|  ********331      |DB|07042026|085034||0|0|00|";
        protected const string DetailsDebitSaleTwoResponsePayload =
            "0261|00|597029414300|IT750050|ABC123|388892|7000|3331|000144|DB|000000|  ********331      |DB|07042026|085628||0|0|00|";
        protected const string DetailsNoSalesResponsePayload =
            "0261|11||||||||||||||||||";

        protected POSIntegradoE2ETestBase()
        {
            _mockHandler = new MockSerialHandler();
            _service = new PosService(_mockHandler);
            _pos = new POSIntegrado.POSIntegrado(_mockHandler, _service);
        }

        protected void AssertSentCommand(string payload)
        {
            Assert.Equal(TestFrameBuilder.BuildCommandFrame(payload), _mockHandler.WrittenData.Single());
        }

        protected void SendAck()
        {
            _mockHandler.SimulateIncoming(ACK.ToString());
        }

        protected void SendResponse(string payload)
        {
            _mockHandler.SimulateIncoming(TestFrameBuilder.BuildCommandFrame(payload));
        }

        protected void AssertFinalAckWritten(int expectedWriteCount)
        {
            Assert.Equal(expectedWriteCount, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        protected static void AssertBaseResponseText(string responseText, string functionCode, int responseCode)
        {
            Assert.Contains($"Function: {functionCode}", responseText);
            Assert.Contains($"Response code:{responseCode}", responseText);
        }

        protected static void AssertVoucherLinesHaveFixedWidth(IReadOnlyList<string> printingField)
        {
            Assert.NotEmpty(printingField);
            Assert.All(printingField, line => Assert.Equal(40, line.Length));
        }
    }
}
