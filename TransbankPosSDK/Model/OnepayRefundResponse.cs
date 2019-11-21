using Transbank.Onepay.Model;

namespace Transbank.POS.Model
{
    public class OnepayRefundResponse
    {
        private readonly RefundCreateResponse response;
        public string ExternalUniqueNumber { get => response.ExternalUniqueNumber; }
        public string Occ { get => response.Occ; }
        public string IssuedAt { get => response.IssuedAt.ToString();}
        public string ReverseCode { get => response.ReverseCode; }
        public string Signature { get => response.Signature; }

        public OnepayRefundResponse(RefundCreateResponse response)
        {
            this.response = response;
        }
    }
}
