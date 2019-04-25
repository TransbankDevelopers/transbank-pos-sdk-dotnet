using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankSaleException : TransbankException
    {
        public SaleResponse SaleResponse;

        public TransbankSaleException(string message, SaleResponse response) : base(message)
        {
            SaleResponse = response;
        }
    }
}
