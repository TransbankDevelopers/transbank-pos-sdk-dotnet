using System;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.IntegradoResponses
{
    public class MultiCodeLastSaleResponse : LastSaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Voucher", 19},
            { "Change", 20},
            { "CommerceProviderCode", 21 }
        };

        public string Voucher
        {
            get
            {
                return VoucherParser.ExtractRawVoucher(Response, ParameterMap["Voucher"]).Trim();
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
                long.TryParse(Response.Split('|')[ParameterMap["CommerceProviderCode"]].Trim(), out long commerceProviderCode);
                return commerceProviderCode;
            }
        }

        public MultiCodeLastSaleResponse(string response) : base(response) { }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Voucher: " + Voucher + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
