namespace Transbank.Responses.CommonResponses
{
    public class LoadKeysResponse : BasicResponse
    {
        public long? CommerceCode { get; }
        public string TerminalId { get; }

        public LoadKeysResponse(string response) : base(response)
        {
            CommerceCode = GetOptionalLongSegment(2, "CommerceCode");
            TerminalId = GetOptionalStringSegment(3);
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
