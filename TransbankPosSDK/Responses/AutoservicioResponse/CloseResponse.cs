using System;
using System.Collections.Generic;
using Transbank.Utils;

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
                return VoucherParser.ParsePrintingField(RawVoucher);
            }
        }

        public string RawVoucher
        {
            get
            {
                return VoucherParser.ExtractRawVoucher(Response, ParameterMap["PrintingField"]);
            }
        }

        public CloseResponse(string response) : base(response) { }

        public override string ToString()
        {
            string printingFieldText = VoucherParser.FormatPrintingField(PrintingField);
            return base.ToString() + "\n" +
                   "Printing Field: " + "\n" + printingFieldText;
        }
    }
}
