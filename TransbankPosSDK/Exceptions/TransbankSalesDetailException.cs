using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankSalesDetailException : TransbankException
    {
        public DetailResponse DetailResponse;

        public TransbankSalesDetailException(string message, DetailResponse response) : base(message)
        {
            DetailResponse = response;
        }
    }
}
