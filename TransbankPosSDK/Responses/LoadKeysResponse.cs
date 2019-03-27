using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class LoadKeysResponse
    {
        public LoadKeyCloseResponse Response {get;}

        public int FunctionCode => Response.function;
        public string Result => ResponseCodes.Map[Response.responseCode];
        public bool Sucess => ResponseCodes.Map[0].Equals(Result);
        public long CommerceCode => Response.commerceCode;
        public int TerminalId => Response.terminalId;

        public LoadKeysResponse(LoadKeyCloseResponse cresponse)
        {
            Response = cresponse;
        }
    }
}
