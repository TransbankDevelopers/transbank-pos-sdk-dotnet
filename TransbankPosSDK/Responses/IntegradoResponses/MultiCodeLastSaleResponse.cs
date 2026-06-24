using Transbank.Utils;

namespace Transbank.Responses.IntegradoResponses
{
    public class MultiCodeLastSaleResponse : LastSaleResponse
    {
        public string Voucher { get; }
        public int? Change { get; }
        public long? CommerceProviderCode { get; }

        public MultiCodeLastSaleResponse(string response) : base(response)
        {
            Voucher = VoucherParser.ExtractRawVoucher(Response, 19).Trim();
            Change = GetOptionalIntSegment(20, "Change");
            CommerceProviderCode = GetOptionalLongSegment(21, "CommerceProviderCode");
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Voucher: " + Voucher + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
