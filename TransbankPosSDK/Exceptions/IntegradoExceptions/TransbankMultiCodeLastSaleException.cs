using System;
using Transbank.POS.IntegradoResponses;

namespace Transbank.POS.IntegradoExceptions
{
    public class TransbankMultiCodeLastSaleException : TransbankMultiCodeSaleException
    {
        public TransbankMultiCodeLastSaleException(string message, LastSaleResponse response) : base(message, response) { }

        public TransbankMultiCodeLastSaleException(string message, Exception inner) : base(message, inner) { }
    }
}
