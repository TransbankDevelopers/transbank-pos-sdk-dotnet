using System;
using Transbank.Responses.CommonResponses;

namespace Transbank.Responses.AutoservicioResponse
{
    public class InitializationResponse : BasicResponse
    {
        public DateTime? RealDate { get; }

        public InitializationResponse(string response) : base(response)
        {
            RealDate = GetOptionalCombinedDateTimeSegment(2, 3, "ddMMyyyyHHmmss", "RealDate");
        }

        public override string ToString()
        {
            string formattedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return base.ToString() + "\n" +
                   "Success?: " + Success + "\n" +
                   "Real Date: " + formattedRealDate;
        }
    }
}
