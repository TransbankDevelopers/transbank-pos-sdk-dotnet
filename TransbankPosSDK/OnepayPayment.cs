using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using Transbank.Onepay;
using Transbank.Onepay.Enums;
using Transbank.Onepay.Model;
using Transbank.POS.Model;

namespace Transbank.POS
{
    public class OnepayPayment : IOnepayPayment
    {
        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler<NewMessageEventArgs> OnNewMessage;
        public event EventHandler<SuccessfulPaymentEventArgs> OnSuccessfulPayment;
        public event EventHandler<NewMessageEventArgs> OnUnsuccessfulPayment;

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
            var env_type = Environment.GetEnvironmentVariable("TBK_ONEPAY_ENVIRONMENT");
            if (env_type == "Live")
            {
                Onepay.Onepay.IntegrationType = OnepayIntegrationType.Live;

                var env_apikey = Environment.GetEnvironmentVariable("TBK_ONEPAY_APIKEY");
                if (!string.IsNullOrEmpty(env_apikey))
                {
                    Onepay.Onepay.ApiKey = env_apikey;
                }

                var env_secret = Environment.GetEnvironmentVariable("TBK_ONEPAY_SHAREDSECRET");
                if (!string.IsNullOrEmpty(env_secret))
                {
                    Onepay.Onepay.SharedSecret = env_secret;
                }
            }

            Ticket = ticket;
            Total = total;
            ExternalUniqueNumber = externalUniqueNumber;
        }

        public void Connected()
        {
            OnConnect?.Invoke(this, new EventArgs());
        }

        public void Disconnected()
        {
            OnDisconnect?.Invoke(this, new EventArgs());
            ws.mqttClient.DisconnectAsync();
        }

        public void NewMessage(string payload)
        {
            WebsocketMessage message = JsonConvert.DeserializeObject<WebsocketMessage>(payload);
            var eventArgs = new NewMessageEventArgs(message);
            OnNewMessage?.Invoke(this, eventArgs);

            switch (message.status)
            {
                case "AUTHORIZED":
                   if (ws.mqttClient.IsConnected)
                    {
                        var response = Transaction.Commit(Occ, ExternalUniqueNumber);
                        OnSuccessfulPayment?.Invoke(this, new SuccessfulPaymentEventArgs(response, ExternalUniqueNumber));
                        ws.mqttClient.DisconnectAsync();
                    }
                    break;

                case "REJECTED_BY_USER":
                case "AUTHORIZATION_ERROR":
                    OnUnsuccessfulPayment?.Invoke(this, eventArgs);
                    ws.mqttClient.DisconnectAsync();
                    break;
                default:
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

        public void StopPayment()
        {
            ws.mqttClient.DisconnectAsync();
        }
    }
}
