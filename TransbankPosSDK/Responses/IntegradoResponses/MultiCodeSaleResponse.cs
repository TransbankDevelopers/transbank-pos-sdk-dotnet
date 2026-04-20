namespace Transbank.Responses.IntegradoResponses
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        public string Filler { get; }
        public int? Change { get; }
        public long? CommerceProviderCode { get; }

        public MultiCodeSaleResponse(string response) : base(response)
        {
            Filler = GetOptionalStringSegment(19);
            Change = GetOptionalIntSegment(20, "Change");
            CommerceProviderCode = GetOptionalLongSegment(21, "CommerceProviderCode");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
