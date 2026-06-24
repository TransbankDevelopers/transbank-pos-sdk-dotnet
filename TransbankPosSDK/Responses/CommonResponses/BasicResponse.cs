using System;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.CommonResponses
{
    public class BasicResponse
    {
        const int APPROVED_RESPONSE_CODE = 0;
        const int INITIALIZATION_OK_RESPONSE_CODE = 90;
        const string MESSAGE_NOT_FOUND = "Mensaje no encontrado";

        private readonly List<string> _parseErrors;
        private readonly string[] _segments;

        public string Response { get; }
        public string RawResponse => Response;
        public string FunctionCode { get; }
        public int? ResponseCode { get; }
        public bool HasValidParse => _parseErrors.Count == 0;
        public IReadOnlyList<string> ParseErrors { get; }
        public virtual bool Success => ResponseCode == APPROVED_RESPONSE_CODE || ResponseCode == INITIALIZATION_OK_RESPONSE_CODE;
        public string ResponseMessage => ResponseCode.HasValue && ResponseCodes.Map.TryGetValue(ResponseCode.Value, out string responseMessage)
            ? responseMessage
            : MESSAGE_NOT_FOUND;

        public BasicResponse(string response)
        {
            Response = response;
            _parseErrors = new List<string>();
            ParseErrors = _parseErrors.AsReadOnly();
            _segments = ResponseParsingHelper.SplitSegments(response, _parseErrors);

            FunctionCode = ResponseParsingHelper.GetRequiredString(_segments, 0, "FunctionCode", _parseErrors);
            ResponseCode = ResponseParsingHelper.GetRequiredInt(_segments, 1, "ResponseCode", _parseErrors);
        }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                    "Response code:" + ResponseCode + "\n" +
                    "Response message: " + ResponseMessage;
        }

        protected string GetOptionalStringSegment(int index)
        {
            return ResponseParsingHelper.GetOptionalString(_segments, index);
        }

        protected string GetRequiredStringSegment(int index, string fieldName)
        {
            return ResponseParsingHelper.GetRequiredString(_segments, index, fieldName, _parseErrors);
        }

        protected int? GetOptionalIntSegment(int index, string fieldName)
        {
            return ResponseParsingHelper.GetOptionalInt(_segments, index, fieldName, _parseErrors);
        }

        protected long? GetOptionalLongSegment(int index, string fieldName)
        {
            return ResponseParsingHelper.GetOptionalLong(_segments, index, fieldName, _parseErrors);
        }

        protected long? GetRequiredLongSegment(int index, string fieldName)
        {
            return ResponseParsingHelper.GetRequiredLong(_segments, index, fieldName, _parseErrors);
        }

        protected DateTime? GetOptionalDateSegment(int index, string format, string fieldName)
        {
            return ResponseParsingHelper.GetOptionalDate(_segments, index, format, fieldName, _parseErrors);
        }

        protected DateTime? GetOptionalCombinedDateTimeSegment(int dateIndex, int timeIndex, string format, string fieldName)
        {
            return ResponseParsingHelper.GetOptionalCombinedDateTime(_segments, dateIndex, timeIndex, format, fieldName, _parseErrors);
        }
    }
}
