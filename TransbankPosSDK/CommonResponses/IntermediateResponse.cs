using System;

namespace Transbank.POS.CommonResponses
{
    public class IntermediateResponse : EventArgs
    {
        private readonly BasicResponse message;

        public int FunctionCode => message.FunctionCode;
        public string ResponseMessage => message.ResponseMessage;
        public int ResponseCode => message.ResponseCode;

        public IntermediateResponse(string response) => message = new BasicResponse(response);
    }
}
