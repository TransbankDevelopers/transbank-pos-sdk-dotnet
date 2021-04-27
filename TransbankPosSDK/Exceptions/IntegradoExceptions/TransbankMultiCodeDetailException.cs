using System;

namespace Transbank.Exceptions.IntegradoExceptions
{
    public class TransbankMultiCodeDetailException : TransbankSalesDetailException
    {
        public TransbankMultiCodeDetailException(string message, Exception inner) : base(message, inner) { }
    }
}
