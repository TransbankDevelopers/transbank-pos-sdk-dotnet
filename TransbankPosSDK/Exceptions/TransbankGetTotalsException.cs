using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankGetTotalsException : TransbankException
    {
        public TotalsResponse GetTotalsResponse;

        public TransbankGetTotalsException(string message, TotalsResponse response) : base(message)
        {
            GetTotalsResponse = response;
        }
    }
}
