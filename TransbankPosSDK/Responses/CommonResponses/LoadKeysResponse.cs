namespace Transbank.Responses.CommonResponses
{
    public class LoadKeysResponse : BasicResponse
    {
        public long? CommerceCode { get; }
        public string TerminalId { get; }

        public LoadKeysResponse(string response) : base(response)
        {
            CommerceCode = GetRequiredLongSegment(2, "CommerceCode");
            TerminalId = GetRequiredStringSegment(3, "TerminalId");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                    "Success?: " + Success + "\n" +
                    "Commerce Code: " + CommerceCode + "\n" +
                    "Terminal Id: " + TerminalId;
        }
    }
}
