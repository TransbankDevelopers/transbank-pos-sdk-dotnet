using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Transbank.POS.Responses.CommonResponses;

namespace Transbank.POS.Responses.AutoservicioResponse
{
    public class InitializationResponse : BasicResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "RealDate", 2},
            { "RealTime", 3}
        };

        public DateTime? RealDate
        {
            get
            {
                string date = "";
                string hour = "";
                try
                {
                    date = Response.Split('|')[ParameterMap["RealDate"]].Trim();
                    hour = Response.Split('|')[ParameterMap["RealTime"]].Trim();
                }
                catch (IndexOutOfRangeException) { }

                if (date + hour != "")
                {
                    DateTime parsedDate = new DateTime();
                    DateTime.TryParseExact(date + hour, "ddMMyyyyHHmmss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out parsedDate);
                    return parsedDate;
                }
                return null;
            }
        }

        public InitializationResponse(string response) : base(response) { }

        public override string ToString()
        {
            string formatedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return base.ToString() + "\n" +
                   "Success?: " + Success + "\n" +
                   "Real Date: " + formatedRealDate;
        }
    }
}
