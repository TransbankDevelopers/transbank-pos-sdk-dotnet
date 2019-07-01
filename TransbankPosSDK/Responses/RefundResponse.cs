using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class RefundResponse
    {
        public RefundCResponse Response { get; }

        public int FunctionCode => Response.function;
        public int ResponseCode => Response.responseCode;
        public long CommerceCode => Response.commerceCode;
        public string TerminalId => Response.terminalId;
        public string AuthorizationCode => Response.authorizationCode;
        public int OperationID => Response.operationID;
        public int Initialized => Response.initilized;

        public string ResponseMessage => ResponseCodes.Map[Response.responseCode];
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage);

        public RefundResponse(RefundCResponse cresponse)
        {
            Response = cresponse;
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "AuthorizationCode: " + AuthorizationCode + "\n" +
                   "OperationID: " + OperationID;
        }
    }
}
