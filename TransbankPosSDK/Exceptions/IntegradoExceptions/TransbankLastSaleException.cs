using System;
using Transbank.Responses.IntegradoResponses;

namespace Transbank.Exceptions.IntegradoExceptions
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
