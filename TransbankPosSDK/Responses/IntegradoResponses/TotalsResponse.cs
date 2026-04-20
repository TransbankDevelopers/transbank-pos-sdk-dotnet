namespace Transbank.Responses.IntegradoResponses
{
    public class TotalsResponse : CommonResponses.BasicResponse
    {
        public int? TxCount { get; }
        public int? TxTotal { get; }

        public TotalsResponse(string response) : base(response)
        {
            TxCount = GetOptionalIntSegment(2, "TxCount");
            TxTotal = GetOptionalIntSegment(3, "TxTotal");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "TX Count: " + TxCount + "\n" +
                   "TX Total: " + TxTotal;
        }
    }
}
