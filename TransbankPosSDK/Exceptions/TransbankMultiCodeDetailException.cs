using System;

namespace Transbank.POS.Exceptions
{
    public class TransbankMultiCodeDetailException : TransbankSalesDetailException
    {
        public TransbankMultiCodeDetailException(string message, Exception inner) : base(message, inner) { }
    }
}
