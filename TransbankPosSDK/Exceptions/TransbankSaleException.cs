using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankSaleException : TransbankException
    {
        public SaleResponse SaleResponse;

        public TransbankSaleException(string message, SaleResponse response) : base(message)
        {
            SaleResponse = response;
        }

        public TransbankSaleException(string message, Exception inner) : base(message, inner) { }

        public TransbankSaleException(string message) : base(message) { }
    }
}
