using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankCloseException : TransbankException
    {
        public CloseResponse RegisterCloseResponse;

        public TransbankCloseException(string message, CloseResponse response) : base(message)
        {
            RegisterCloseResponse = response;
        }
    }
}
