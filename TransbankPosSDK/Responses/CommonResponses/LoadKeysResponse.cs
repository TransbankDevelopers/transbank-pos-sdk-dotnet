using Transbank.Utils;
using System.Collections.Generic;
using System;

namespace Transbank.Responses.CommonResponses
{
    public class LoadKeysResponse : BasicResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "CommerceCode", 2},
            { "TerminalId", 3}
        };

        public long CommerceCode
        {
            get
            {
                try
                {
                    _ = long.TryParse(Response.Split('|')[ParameterMap["CommerceCode"]].Trim(), out long commerceCode);
                    return commerceCode;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public string TerminalId
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["TerminalId"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }

        public LoadKeysResponse(string response) : base(response) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                    "Success?: " + Success + "\n" +
                    "Commerce Code: " + CommerceCode + "\n" +
                    "Terminal Id: " + TerminalId;
        }
    }
}
