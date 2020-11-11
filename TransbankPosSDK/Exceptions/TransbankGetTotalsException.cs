using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    [Obsolete("This class is not longe used, Please use TransbankTotalsException instead", error: false)]
    public class TransbankGetTotalsException : TransbankTotalsException
    {
        public TransbankGetTotalsException(string message, TotalsResponse response) : base(message, response) { }

        public TransbankGetTotalsException(string message, Exception inner) : base(message, inner) { }
    }
}
