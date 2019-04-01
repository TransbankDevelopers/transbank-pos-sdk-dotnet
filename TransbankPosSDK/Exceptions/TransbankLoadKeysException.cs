using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankLoadKeysException : TransbankException
    {
        public LoadKeysResponse LoadKeyResponse;

        public TransbankLoadKeysException(string message, LoadKeysResponse response) : base(message)
        {
            LoadKeyResponse = response;
        }
    }
}
