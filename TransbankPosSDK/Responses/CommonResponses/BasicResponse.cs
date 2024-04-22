using Transbank.Utils;
using System.Collections.Generic;

namespace Transbank.Responses.CommonResponses
{
    public class BasicResponse
    {
        const byte APPROVED_RESPONSE_CODE = 0;
        const byte INITIALIZATION_OK_RESPONSE_CODE = 90;

        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "FunctionCode", 0},
            { "ResponseCode", 1},
        };

        public string Response { get; }

        public string FunctionCode
        {
            get
            {
                return Response.Split('|')[ParameterMap["FunctionCode"]].Trim();
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
        public bool Success => ResponseCodes.Map[APPROVED_RESPONSE_CODE].Equals(ResponseMessage) || ResponseCodes.Map[INITIALIZATION_OK_RESPONSE_CODE].Equals(ResponseMessage);

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
