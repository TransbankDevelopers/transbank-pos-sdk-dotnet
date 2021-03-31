﻿using System;

namespace Transbank.POS.Exceptions
{
    public class TransbankException : Exception
    {
        public TransbankException() : base() { }
        public TransbankException(string message) : base(message) { }
        public TransbankException(string message, Exception inner) : base(message, inner) { }
    }
}
