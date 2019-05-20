using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankGetTotalsException : TransbankException
    {
        public GetTotalsResponse GetTotalsResponse;

        public TransbankGetTotalsException(string message, GetTotalsResponse response) : base(message)
        {
            GetTotalsResponse = response;
        }
    }
}
