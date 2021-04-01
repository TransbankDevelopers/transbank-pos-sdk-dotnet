using System;
using Transbank.POS.Responses.IntegradoResponses;

namespace Transbank.POS.Exceptions.IntegradoExceptions
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
