using System;

namespace Transbank.Exceptions.CommonExceptions
{
    public class IntermediateResponseException : TransbankException
    {
        public IntermediateResponseException() : base() { }
        public IntermediateResponseException(string message) : base(message) { }
        public IntermediateResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
