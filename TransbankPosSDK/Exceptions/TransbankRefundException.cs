using System;
using Transbank.POS.Responses;

namespace Transbank.POS.Exceptions
{
    public class TransbankRefundException : TransbankException
    {
        public RefundResponse RefundResp;

        public TransbankRefundException(string message, RefundResponse response) : base(message)
        {
            RefundResp = response;
        }

        public TransbankRefundException(string message, Exception inner) : base(message, inner) { }
    }
}
