using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Filler", 19},
            { "Change", 20},
            { "CommerceProviderCode", 21 }
        };

        public string Filler
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["Filler"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public int Change
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Change"]].Trim(), out int amount);
                return amount;
            }
        }
        public long CommerceProviderCode
        {
            get
            {
                long.TryParse(Response.Split('|')[ParameterMap["CommerceProviderCode"]].Trim(), out long SharesNumber);
                return SharesNumber;
            }
        }

        public MultiCodeSaleResponse(string response) : base(response) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
