namespace Transbank.Responses.IntegradoResponses
{
    public class MultiCodeDetailResponse : SaleResponse
    {
        public int? Change { get; }
        public long? CommerceProviderCode { get; }

        public MultiCodeDetailResponse(string detail) : base(detail)
        {
            Change = GetOptionalIntSegment(19, "Change");
            CommerceProviderCode = GetOptionalLongSegment(20, "CommerceProviderCode");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
