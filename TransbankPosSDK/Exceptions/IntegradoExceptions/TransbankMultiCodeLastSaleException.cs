using System;
using Transbank.Responses.IntegradoResponses;

namespace Transbank.Exceptions.IntegradoExceptions
{
    public class TransbankMultiCodeLastSaleException : TransbankMultiCodeSaleException
    {
        public TransbankMultiCodeLastSaleException(string message, LastSaleResponse response) : base(message, response) { }

        public TransbankMultiCodeLastSaleException(string message, Exception inner) : base(message, inner) { }
    }
}
