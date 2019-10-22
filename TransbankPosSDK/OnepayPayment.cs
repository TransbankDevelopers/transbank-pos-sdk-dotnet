using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Transbank.Onepay;
using Transbank.Onepay.Enums;
using Transbank.Onepay.Model;
using Transbank.Onepay.Exceptions;
using Transbank.POS.Model;

namespace Transbank.POS
{
    public class OnepayPayment : IOnepayPayment
    {
        private ShoppingCart cart;
        private Websocket ws;

        public int Ticket { get; }
        public int Total { get; }
        public string ExternalUniqueNumber { get; }

        public string Occ { get; private set; }
        public string Ott { get; private set; }

        public OnepayPayment(int ticket, int total): 
            this (ticket, total, Guid.NewGuid().ToString()){}

        public OnepayPayment(int ticket, int total, string externalUniqueNumber)
        {
            Onepay.Onepay.CallbackUrl = "https://www.continuum.cl";
            Ticket = ticket;
            Total = total;
            ExternalUniqueNumber = externalUniqueNumber;
        }

        public void Connected()
        {
        }

        public void Disconnected()
        {
        }

        public void NewMessage(string payload)
        {
        }

        public OnepayCreateResponse StarPayment()
        {
            cart = new ShoppingCart();
            cart.Add(new Item(Ticket.ToString(), 1, Total, "", -1));

            var response = Transaction.Create(cart, ChannelType.Mobile, ExternalUniqueNumber);
            Occ = response.Occ;
            Ott = response.Ott.ToString();

            return new OnepayCreateResponse(response);
        }

    }
}
