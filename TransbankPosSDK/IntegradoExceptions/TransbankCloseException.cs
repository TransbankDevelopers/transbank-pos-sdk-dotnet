using System;
using Transbank.POS.IntegradoResponses;

namespace Transbank.POS.IntegradoExceptions
{
    public class TransbankCloseException : CommonExceptions.TransbankException
    {
        public CloseResponse RegisterCloseResponse;

        public TransbankCloseException(string message, CloseResponse response) : base(message)
        {
            RegisterCloseResponse = response;
        }

        public TransbankCloseException(string message, Exception inner) : base(message, inner) { }

        public TransbankCloseException(string message) : base(message) { }
    }
}
