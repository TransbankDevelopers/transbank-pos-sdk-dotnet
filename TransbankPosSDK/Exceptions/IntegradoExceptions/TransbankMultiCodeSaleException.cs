using System;
using Transbank.POS.IntegradoResponses;

namespace Transbank.POS.IntegradoExceptions
{
    public class TransbankMultiCodeSaleException : TransbankSaleException
    {
        public TransbankMultiCodeSaleException(string message, SaleResponse response) : base(message)
        {
            SaleResponse = response;
        }

        public TransbankMultiCodeSaleException(string message, Exception inner) : base(message, inner) { }

        public TransbankMultiCodeSaleException(string message) : base(message) { }
    }
}
