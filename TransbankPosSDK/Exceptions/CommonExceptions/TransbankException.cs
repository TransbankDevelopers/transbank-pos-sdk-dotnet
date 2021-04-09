﻿using System;

namespace Transbank.Exceptions.CommonExceptions
{
    public class TransbankException : Exception
    {
        public TransbankException() : base() { }
        public TransbankException(string message) : base(message) { }
        public TransbankException(string message, Exception inner) : base(message, inner) { }
    }
}
