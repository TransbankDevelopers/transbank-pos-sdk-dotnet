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

        public List<string> PrintingField
        {
            get
            {
                List<string> printingField = new List<string>();

                try
                {
                    string[] arrayResponse = Response.Split('|');
                    if (Response.Split('|').Length < 5)
                    {
                        printingField.Add("");
                        return printingField;
                    }

                    string response = arrayResponse[ParameterMap["PrintingField"]];

                    if (response.Length % 40 != 0)
                    {
                        printingField.Add(response);
                        return printingField;
                    }

                    for (int i = 0; i < response.Length; i += 40)
                        printingField.Add(response.Substring(i, 40));
                 
                    return printingField;
                }
                catch (IndexOutOfRangeException)
                {
                    return printingField;
                }
            }
        }

        public CloseResponse(string response) : base(response) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Printing Field: " + ((PrintingField.Count > 1) ? "\n" + string.Join("\n", PrintingField) : PrintingField[0]);
        }
    }
}
