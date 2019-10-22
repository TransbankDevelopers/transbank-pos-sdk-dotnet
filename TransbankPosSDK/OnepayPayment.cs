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
            WebsocketMessage message = JsonConvert.DeserializeObject<WebsocketMessage>(payload);
            var eventArgs = new NewMessageEventArgs(message);

            Console.WriteLine("Occ: " + Occ);

            switch (message.status)
            {
                case "AUTHORIZED":
                    try
                    {
                        var response = Transaction.Commit(Occ, ExternalUniqueNumber);
                        ws.mqttClient.DisconnectAsync();
                    }
                    catch
                    {
                        ws.mqttClient.DisconnectAsync();
                        throw ;
                    }
                    break;

                case "REJECTED_BY_USER":
                case "AUTHORIZATION_ERROR":
                    ws.mqttClient.DisconnectAsync();
                    break;

                default:
                    Console.WriteLine("Default: " + message.status + "\ndesc: " + message.description);
                    break;
            }
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

        public async Task WatchPayment(OnepayCreateResponse response)
        {
            ws = new Websocket();
            await ws.Connect(this);
        }
    }
}
