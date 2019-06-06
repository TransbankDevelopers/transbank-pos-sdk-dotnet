using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankCancellationException : TransbankException
    {
        public CancellResponse CancellationResp;

        public TransbankCancellationException(string message, CancellResponse response) : base(message)
        {
            CancellationResp = response;
        }
    }
}
