using System;
using Transbank.Onepay.Model;

namespace Transbank.POS.Model
{
    public class NewMessageEventArgs : EventArgs
    {
        private readonly WebsocketMessage Message;

        public string Status { get => Message.status; }
        public string Description { get => Message.description; }

        public NewMessageEventArgs(WebsocketMessage payload)
        {
            Message = payload;
        }

        public override string ToString()
        {
            return "Message: " + Message.ToString();
        }
    }
}
