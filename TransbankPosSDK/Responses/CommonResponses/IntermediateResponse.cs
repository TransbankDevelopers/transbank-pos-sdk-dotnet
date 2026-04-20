using System;
using System.Collections.Generic;

namespace Transbank.Responses.CommonResponses
{
    public class IntermediateResponse : EventArgs
    {
        const string INTERMEDIATE_FUNCTION_CODE = "0900";
        private readonly BasicResponse _message;

        public string Response => _message.Response;
        public string RawResponse => _message.RawResponse;
        public string FunctionCode => _message.FunctionCode;
        public string ResponseMessage => _message.ResponseMessage;
        public int? ResponseCode => _message.ResponseCode;
        public bool HasValidParse => _message.HasValidParse;
        public IReadOnlyList<string> ParseErrors => _message.ParseErrors;
        public bool Success => HasValidParse && FunctionCode == INTERMEDIATE_FUNCTION_CODE && ResponseCode.HasValue;

        public IntermediateResponse(string response)
        {
            _message = new BasicResponse(response);
        }
    }
}
