using Transbank.POS.Utils;
using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class TotalsResponse : BasicResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "TxCount", 2},
            { "TxTotal", 3},
        };
        public int TxCount
        {
            get
            {
                _ = int.TryParse(Response.Split('|')[ParameterMap["TxCount"]].Trim(), out int Count);
                return Count;
            }
        }
        public int TxTotal
        {
            get
            {
                _ = int.TryParse(Response.Split('|')[ParameterMap["TxTotal"]].Trim(), out int Total);
                return Total;
            }
        }

        public TotalsResponse(string response) : base(response) { }

        public override string ToString()
        {
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "TX Count: " + TxCount + "\n" +
                   "TX Total: " + TxTotal;
        }
    }
}
