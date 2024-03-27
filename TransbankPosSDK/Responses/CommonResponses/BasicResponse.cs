using Transbank.Utils;
using System.Collections.Generic;

namespace Transbank.Responses.CommonResponses
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
                _ = int.TryParse(Response.Split('|')[ParameterMap["FunctionCode"]].Trim(), out int functionCode);
                return functionCode;
            }
        }
        public string ResponseMessage
        {
            get
            {
                try
                {
                    return ResponseCodes.Map[ResponseCode];
                }
                catch (KeyNotFoundException)
                {
                    return "Mensaje no encontrado";
                }
            }
        }
        public int ResponseCode
        {
            get
            {
                _ = int.TryParse(Response.Split('|')[ParameterMap["ResponseCode"]].Trim(), out int responseCode);
                return responseCode;
            }
        }
        public bool Success => ResponseCodes.Map[0].Equals(ResponseMessage) || ResponseCodes.Map[90].Equals(ResponseMessage);

        public BasicResponse(string response)
        {
            Response = response;
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                    "Response code:" + ResponseCode + "\n" +
                    "Response message: " + ResponseMessage;
        }
    }
}
