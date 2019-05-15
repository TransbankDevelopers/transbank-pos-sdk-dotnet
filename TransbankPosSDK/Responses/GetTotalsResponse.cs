using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class GetTotalsResponse
    {
        public TotalsResponse Response { get; }

        public int FunctionCode => Response.function;
        public int ResponseCode => Response.responseCode;
        public int TxCount => Response.txCount;
        public int TxTotal => Response.txTotal;
        public int Initialized => Response.initilized;

        public string ResponseMessage => ResponseCodes.Map[Response.responseCode];
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage);

        public GetTotalsResponse(TotalsResponse cresponse)
        {
            Response = cresponse;
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseCode + "\n" +
                   "TX Count: " + TxCount + "\n" +
                   "TX Total: " + TxTotal;
        }
    }
}
