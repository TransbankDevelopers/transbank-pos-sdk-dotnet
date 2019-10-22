using System;
using System.Collections.Generic;
using System.Text;
using Transbank.Onepay.Model;

namespace Transbank.POS.Model
{
    public class OnepayCreateResponse
    {
        private readonly TransactionCreateResponse response;
        public string Ott { get => response.Ott.ToString(); }
        public string Occ { get => response.Occ; }
        public string ExternalUniqueNumber { get => response.ExternalUniqueNumber; }
        public string QrCodeAsBase64 { get => response.QrCodeAsBase64; }

        public OnepayCreateResponse(TransactionCreateResponse response)
        {
            this.response = response;
        }
    }
}
