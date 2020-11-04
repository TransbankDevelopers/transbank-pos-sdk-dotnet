using System.Collections.Generic;
using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class BasicResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "FunctionCode", 0},
            { "ResponseCode", 1},
        };

        public string Response { get; }

        public int FunctionCode
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["FunctionCode"]].Trim(), out int functionCode);
                return functionCode;
            }
        }
        public string ResponseMessage => ResponseCodes.Map[ResponseCode];
        public int ResponseCode
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["ResponseCode"]].Trim(), out int responseCode);
                return responseCode;
            }
        }

        public BasicResponse(string response)
        {
            Response = response.Substring(1);
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                    "Response: " + ResponseMessage + "\n";
        }
    }
}
