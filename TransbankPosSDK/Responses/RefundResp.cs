using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class RefundResp
    {
        public RefundResponse Response { get; }

        public int FunctionCode => Response.function;
        public int ResponseCode => Response.responseCode;
        public long CommerceCode => Response.commerceCode;
        public int TerminalId => Response.terminalId;
        public int AuthorizationCode => Response.authorizationCode;
        public int OperationID => Response.operationID;
        public int Initialized => Response.initilized;

        public string ResponseMessage => ResponseCodes.Map[Response.responseCode];
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage);

        public RefundResp(RefundResponse cresponse)
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
