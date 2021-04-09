using System;
using Transbank.Responses.CommonResponses;

namespace Transbank.Exceptions.CommonExceptions
{
    public class TransbankRefundException : CommonExceptions.TransbankException
    {
        public RefundResponse RefundResp;

        public TransbankRefundException(string message, RefundResponse response) : base(message)
        {
            RefundResp = response;
        }

        public TransbankRefundException(string message, Exception inner) : base(message, inner) { }
    }
}
