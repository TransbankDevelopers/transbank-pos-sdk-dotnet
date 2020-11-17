using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class MultiCodeDetailResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Change", 19},
            { "CommerceProviderCode", 20}
        };

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

        public MultiCodeDetailResponse(string detail) : base(detail) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
