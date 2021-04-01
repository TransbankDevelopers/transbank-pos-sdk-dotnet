using System;
using Transbank.POS.Responses.IntegradoResponses;

namespace Transbank.POS.Exceptions.IntegradoExceptions
{
    [Obsolete("This class is not longe used, Please use TransbankTotalsException instead", error: false)]
    public class TransbankGetTotalsException : TransbankTotalsException
    {
        public TransbankGetTotalsException(string message, TotalsResponse response) : base(message, response) { }

        public TransbankGetTotalsException(string message, Exception inner) : base(message, inner) { }
    }
}
