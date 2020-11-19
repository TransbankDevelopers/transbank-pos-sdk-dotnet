using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankLastSaleException : TransbankException
    {
        public LastSaleResponse LastSaleResponse;

        public TransbankLastSaleException(string message, LastSaleResponse response) : base(message)
        {
            LastSaleResponse = response;
        }

        public TransbankLastSaleException(string message, Exception inner) : base(message, inner) { }
    }
}
