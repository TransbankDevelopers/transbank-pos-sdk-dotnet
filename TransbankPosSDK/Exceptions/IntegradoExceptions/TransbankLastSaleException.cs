using System;
using Transbank.POS.Responses.IntegradoResponses;

namespace Transbank.POS.Exceptions.IntegradoExceptions
{
    public class TransbankLastSaleException : CommonExceptions.TransbankException
    {
        public LastSaleResponse LastSaleResponse;

        public TransbankLastSaleException(string message, LastSaleResponse response) : base(message)
        {
            LastSaleResponse = response;
        }

        public TransbankLastSaleException(string message, Exception inner) : base(message, inner) { }
    }
}
