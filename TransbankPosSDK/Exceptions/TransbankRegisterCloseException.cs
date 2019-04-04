using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankRegisterCloseException : TransbankException
    {
        public RegisterCloseResponse RegisterCloseResponse;

        public TransbankRegisterCloseException(string message, RegisterCloseResponse response) : base(message)
        {
            RegisterCloseResponse = response;
        }
    }
}
