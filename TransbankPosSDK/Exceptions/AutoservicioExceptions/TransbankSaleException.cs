using System;
using Transbank.POS.Responses.AutoservicioResponse;

namespace Transbank.POS.Exceptions.AutoservicioExceptions
{
    public class TransbankSaleException : CommonExceptions.TransbankException
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
