﻿using System;
using Transbank.Responses.IntegradoResponses;

namespace Transbank.Exceptions.IntegradoExceptions
{
    public class TransbankSaleException : CommonExceptions.TransbankException
    {
        public SaleResponse SaleResponse;

        public TransbankSaleException(string message, SaleResponse response) : base(message)
        {
            SaleResponse = response;
        }

        public TransbankSaleException(string message, Exception inner) : base(message, inner) { }

        public TransbankSaleException(string message) : base(message) { }
    }
}
