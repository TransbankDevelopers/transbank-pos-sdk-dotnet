namespace Transbank.Responses.CommonResponses
{
    public class RefundResponse : LoadKeysResponse
    {
        public string AuthorizationCode { get; }
        public int? OperationID { get; }

        public RefundResponse(string response) : base(response)
        {
            AuthorizationCode = GetOptionalStringSegment(4);
            OperationID = GetOptionalIntSegment(5, "OperationID");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "AuthorizationCode: " + AuthorizationCode + "\n" +
                   "OperationID: " + OperationID;
        }
    }
}
