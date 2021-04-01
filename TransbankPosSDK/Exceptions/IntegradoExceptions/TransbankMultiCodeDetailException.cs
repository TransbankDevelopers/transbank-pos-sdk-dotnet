using System;

namespace Transbank.POS.Exceptions.IntegradoExceptions
{
    public class TransbankMultiCodeDetailException : TransbankSalesDetailException
    {
        public TransbankMultiCodeDetailException(string message, Exception inner) : base(message, inner) { }
    }
}
