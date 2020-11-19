using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankMultiCodeLastSaleException : TransbankMultiCodeSaleException
    {
        public TransbankMultiCodeLastSaleException(string message, LastSaleResponse response) : base(message, response) { }

        public TransbankMultiCodeLastSaleException(string message, Exception inner) : base(message, inner) { }
    }
}
