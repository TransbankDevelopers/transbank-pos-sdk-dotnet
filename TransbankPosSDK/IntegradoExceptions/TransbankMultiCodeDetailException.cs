using System;

namespace Transbank.POS.IntegradoExceptions
{
    public class TransbankMultiCodeDetailException : TransbankSalesDetailException
    {
        public TransbankMultiCodeDetailException(string message, Exception inner) : base(message, inner) { }
    }
}
