using System;
using Transbank.Responses.CommonResponses;

namespace Transbank.Exceptions.CommonExceptions
{
    public class TransbankLoadKeysException : TransbankException
    {
        public LoadKeysResponse LoadKeyResponse;

        public TransbankLoadKeysException(string message, LoadKeysResponse response) : base(message)
        {
            LoadKeyResponse = response;
        }

        public TransbankLoadKeysException(string message, Exception inner) : base(message, inner) { }
    }
}
