using System;
using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class RefundResponse : LoadKeysResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "AuthorizationCode", 4},
            { "OperationID", 5 }
        };

        public string AuthorizationCode
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["AuthorizationCode"]].Trim();
                }
                catch (IndexOutOfRangeException) {
                    return "none";
                }
            }
        }
        public int OperationID
        {
            get
            {
                _ = int.TryParse(Response.Split('|')[ParameterMap["OperationID"]].Trim(), out int operationID);
                return operationID;
            }
        }

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
