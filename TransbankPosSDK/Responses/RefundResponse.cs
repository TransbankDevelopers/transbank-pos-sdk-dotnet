using System.Collections.Generic;
using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class RefundResponse : BasicResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "CommerceCode", 2},
            { "TerminalId", 3},
            { "AuthorizationCode", 4},
            { "OperationID", 5 }
        };

        public long CommerceCode
        {
            get
            {
                _ = long.TryParse(Response.Split('|')[ParameterMap["CommerceCode"]].Trim(), out long commerceCode);
                return commerceCode;
            }
        }
        public string TerminalId => Response.Split('|')[ParameterMap["TerminalId"]].Trim();
        public string AuthorizationCode => Response.Split('|')[ParameterMap["AuthorizationCode"]].Trim();
        public int OperationID
        {
            get
            {
                _ = int.TryParse(Response.Split('|')[ParameterMap["OperationID"]].Trim(), out int operationID);
                return operationID;
            }
        }
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage);

        public RefundResponse(string response) : base(response) { }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "AuthorizationCode: " + AuthorizationCode + "\n" +
                   "OperationID: " + OperationID;
        }
    }
}
