using System;
using Transbank.Responses.AutoservicioResponse;

namespace Transbank.Exceptions.AutoservicioExceptions
{
    public class TransbankInitializationResponseException : CommonExceptions.TransbankException
    {
        public InitializationResponse RegisterInitializationResponse;

        public TransbankInitializationResponseException(string message, InitializationResponse response) : base(message)
        {
            RegisterInitializationResponse = response;
        }

        public TransbankInitializationResponseException(string message, Exception inner) : base(message, inner) { }

        public TransbankInitializationResponseException(string message) : base(message) { }
    }
}
