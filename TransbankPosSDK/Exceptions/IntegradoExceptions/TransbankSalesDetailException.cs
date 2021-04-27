using System;
using Transbank.Responses.IntegradoResponses;

namespace Transbank.Exceptions.IntegradoExceptions
{
    public class TransbankSalesDetailException : CommonExceptions.TransbankException
    {
        public DetailResponse DetailResponse;

        public TransbankSalesDetailException(string message, DetailResponse response) : base(message)
        {
            DetailResponse = response;
        }

        public TransbankSalesDetailException(string message, Exception inner) : base(message, inner) { }
    }
}
