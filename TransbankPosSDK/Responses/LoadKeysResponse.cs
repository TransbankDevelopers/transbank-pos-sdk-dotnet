using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class LoadKeysResponse
    {
        public LoadKeyCloseResponse Response {get;}

        public int FunctionCode => Response.function;
        public string ResponseMessage => ResponseCodes.Map[Response.responseCode];
        public int ResponseCode => Response.responseCode;
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage);
        public long CommerceCode => Response.commerceCode;
        public int TerminalId => Response.terminalId;

        public LoadKeysResponse(LoadKeyCloseResponse cresponse)
        {
            Response = cresponse;
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                    "Response: " + ResponseMessage + "\n" +
                    "Sucess?: " + Success + "\n" +
                    "Commerce Code: " + CommerceCode + "\n" +
                    "Terminal Id: " + TerminalId;
        }
    }
}
