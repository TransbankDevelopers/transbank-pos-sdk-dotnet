using System;
using Transbank.POS.IntegradoResponses;

namespace Transbank.POS.IntegradoExceptions
{
    public class TransbankTotalsException : CommonExceptions.TransbankException
    {
        public TotalsResponse totalsResponse;

        public TransbankTotalsException(string message, TotalsResponse response) : base(message)
        {
            totalsResponse = response;
        }

        public TransbankTotalsException(string message, Exception inner) : base(message, inner) { }
    }
}
