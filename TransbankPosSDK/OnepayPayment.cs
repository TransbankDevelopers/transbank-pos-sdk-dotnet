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
        public void Connected()
        {
        }

        public void Disconnected()
        {
        }

        public void NewMessage(string payload)
        {
        }

    }
}
