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

                string rawVoucher = RawVoucher;
                if (string.IsNullOrWhiteSpace(rawVoucher) || rawVoucher.Length % 40 != 0)
                {
                    return printingField;
                }

                for (int i = 0; i < rawVoucher.Length; i += 40)
                    printingField.Add(rawVoucher.Substring(i, 40));
                 
                return printingField;
            }
        }

        public string RawVoucher
        {
            get
            {
                try
                {
                    string[] arrayResponse = Response.Split('|');
                    if (arrayResponse.Length <= ParameterMap["PrintingField"])
                    {
                        return string.Empty;
                    }

                    return arrayResponse[ParameterMap["PrintingField"]];
                }
                catch (IndexOutOfRangeException)
                {
                    return string.Empty;
                }
            }
        }

        public CloseResponse(string response) : base(response) { }

        public override string ToString()
        {
            string printingFieldText = PrintingField.Count == 0
                ? ""
                : string.Join("\n", PrintingField);
            return base.ToString() + "\n" +
                   "Printing Field: " + printingFieldText;
        }
    }
}
