using System;
using Transbank.POS.Responses.IntegradoResponses;

namespace Transbank.POS.Exceptions.IntegradoExceptions
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
