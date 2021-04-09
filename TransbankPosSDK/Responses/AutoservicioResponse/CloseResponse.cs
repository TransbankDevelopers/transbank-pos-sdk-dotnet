using System;
using System.Collections.Generic;

namespace Transbank.Responses.AutoservicioResponse
{
    public class CloseResponse : CommonResponses.LoadKeysResponse
    {
        protected Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "PrintingField", 4},
        };

        public string PrintingField
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["PrintingField"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }

        public CloseResponse(string response) : base(response) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Printing Field: " + PrintingField;
        }
    }
}
