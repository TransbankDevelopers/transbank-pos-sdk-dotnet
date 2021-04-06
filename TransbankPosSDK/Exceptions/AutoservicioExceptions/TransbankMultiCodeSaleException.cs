using System;
using Transbank.POS.Responses.AutoservicioResponse;

namespace Transbank.POS.Exceptions.AutoservicioExceptions
{
    public class TransbankMultiCodeSaleException : TransbankSaleException
    {
        public TransbankMultiCodeSaleException(string message, MultiCodeSaleResponse response) : base(message)
        {
            SaleResponse = response;
        }

        public TransbankMultiCodeSaleException(string message, Exception inner) : base(message, inner) { }

        public TransbankMultiCodeSaleException(string message) : base(message) { }
    }
}
