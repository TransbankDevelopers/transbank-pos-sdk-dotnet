using System;
using System.Linq;
using Transbank.Exceptions.CommonExceptions;

namespace Transbank.Responses.CommonResponses
{
    public class IntermediateResponse : EventArgs
    {
        private readonly BasicResponse message;

        public string FunctionCode => message.FunctionCode;
        public string ResponseMessage => message.ResponseMessage;
        public int ResponseCode => message.ResponseCode;

        public IntermediateResponse(string response)
        {
            string[] parts = response.Split('|').Select(part => part.Trim()).ToArray();
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new IntermediateResponseException($"Invalid intermediate response format: {response}");
            }

            message = new BasicResponse(response);
        }
    }
}
