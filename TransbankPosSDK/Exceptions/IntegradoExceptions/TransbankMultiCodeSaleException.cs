using System;
using Transbank.POS.Responses.IntegradoResponses;

namespace Transbank.POS.Exceptions.IntegradoExceptions
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
