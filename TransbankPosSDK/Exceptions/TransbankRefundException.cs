using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankRefundException : TransbankException
    {
        public RefundResp RefundResp;

        public TransbankRefundException(string message, RefundResp response) : base(message)
        {
            RefundResp = response;
        }
    }
}
