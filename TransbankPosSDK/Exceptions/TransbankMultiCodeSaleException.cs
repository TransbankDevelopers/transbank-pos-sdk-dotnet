using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
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
