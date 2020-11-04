using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankTotalsException : TransbankException
    {
        public TotalsResponse totalsResponse;

        public TransbankTotalsException(string message, TotalsResponse response) : base(message)
        {
            totalsResponse = response;
        }

        public TransbankTotalsException(string message, Exception inner) : base(message, inner) { }
    }
}
