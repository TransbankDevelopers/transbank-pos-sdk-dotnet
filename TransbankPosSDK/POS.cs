using System;
using System.IO.Ports;
using Transbank.POS.Utils;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly POS _instance = new POS();
        private readonly int _defaultBaudrate = 115200;
        private string _currentResponse;
        public event EventHandler<IntermediateResponse> IntermediateResponseChange;
        private string CurrentResponse
        {
            get { return _currentResponse; }
            set
            {
                _currentResponse = value;
                if (_currentResponse.Length >= 1 && _currentResponse.Substring(1).Split('|')[0] == "0900")
                {
                    OnIntermediateMessageReceived(CurrentResponse);
                }
            }
        }

        protected virtual void OnIntermediateMessageReceived(string message)
        {
            IntermediateResponseChange?.Invoke(this, new IntermediateResponse(message));
        }

        public SerialPort Port { get; set; }

        private POS()
        {
        }

        public static POS Instance
        {
            get
            {
                return _instance;
            }
        }

        public void OpenPort(string portName) => OpenPort(portName, _defaultBaudrate);

        public void OpenPort(string portName, int baudrate)
        {
            if (Port != null && Port.IsOpen) ClosePort();
            try
            {
                Port = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);
                Port.Open();
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
            }
            catch (Exception e)
            {
                throw new TransbankException("Could not Open Serial Port: " + portName, e);
            }
        }

        public void ClosePort()
        {
            if (!Port.IsOpen) return;
            try
            {
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
                Port.Close();
            }
            catch (Exception e)
            {
                throw new TransbankException("Could not Close Serial Port: " + Port.PortName, e);
            }
        }

        [ObsoleteAttribute("This method is obsolete. Call SaleResponse(int, string) instead.", false)]
        public SaleResponse Sale(int amount, int ticket)
        {
           return Sale(amount, ticket.ToString());
        }

        public LastSaleResponse LastSale()
        {
            if (_configured)
            {
                try
                {
                    LastSaleResponse response = new LastSaleResponse(TransbankWrap.last_sale());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankLastSaleException("LastSale returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankLastSaleException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute LastSale on pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public RefundResponse Refund(int operationID)
        {
            if (_configured)
            {
                try
                {
                    RefundResponse response = new RefundResponse(TransbankWrap.refund(operationID));
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankRefundException("Refund returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankRefundException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to make Refund on POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public TotalsResponse Totals()
        {
            if (_configured)
            {
                try
                {
                    TotalsResponse response = new TotalsResponse(TransbankWrap.get_totals());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankGetTotalsException("Get Totals retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankGetTotalsException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to get totals in POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public List<DetailResponse> Details(bool printOnPOS)
        {
            if (_configured)
            {
                try
                {
                    var details = new List<DetailResponse>();
                    string response = TransbankWrap.sales_detail(printOnPOS);
                    if (response == "")
                    {
                        return details;
                    }

                    string[] lines = response.Split('\n');
                    foreach (string line in lines)
                    {
                        details.Add(new DetailResponse(line));
                    }

                    return details;
                }
                catch (TransbankSalesDetailException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to get details in POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }
        public SaleResponse Sale(int amount, string ticket, bool sendStatus = false)
        {
            string message = $"0200|{amount}|{ticket}|||{Convert.ToInt32(sendStatus)}|";

            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            try
            {
                WriteData(MessageWithLRC(message), intermediateMessages: sendStatus).Wait();
                return new SaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankSaleException($"Unable to execute sale on pos", e);
            }
        }

        public CloseResponse Close()
        {
            try
            {
                WriteData("0500||").Wait();
                return new CloseResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankCloseException("Unable to execute close in pos", e);
            }
        }

        public LoadKeysResponse LoadKeys()
        {
            try
            {
                WriteData("0800").Wait();
                return new LoadKeysResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public bool Poll()
        {
            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to Poll port {Port.PortName} is closed");
            }
            try
            {
                Port.Write("0100");
                Thread.Sleep(500);
                string response = Port.ReadExisting();

                byte[] ba = Encoding.ASCII.GetBytes(response);
                var hexString = BitConverter.ToString(ba);

                return response.Equals("");
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public void SetNormalMode()
        {
            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to Set Normal Mode port {Port.PortName} is closed");
            }
            try
            {
                Port.Write("0300\0");
                ClosePort();
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port {Port.PortName}", e);
            }
        }

        private async Task WriteData(string payload, bool intermediateMessages = false, bool saleDetail = false, bool sendMessageToRegister = false)
        {
            CurrentResponse = "";
            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
            }
            Port.Write(payload);
            ReadAck();
            if (intermediateMessages)
            {
                await ReadMessage(new CancellationTokenSource(90000).Token);
                string responseCode = CurrentResponse.Substring(1).Split('|')[1];
                while (responseCode == "78" || responseCode == "79" ||
                    responseCode == "80" || responseCode == "81" || responseCode == "82")
                {
                    await ReadMessage(new CancellationTokenSource(90000).Token);
                    responseCode = CurrentResponse.Substring(1).Split('|')[1];
                }
            }
            else
            {
                await ReadMessage(new CancellationTokenSource(90000).Token);
            }
        }

        private async Task ReadMessage(CancellationToken token)
        {
            while (!token.IsCancellationRequested && Port.BytesToRead <= 0)
            {
            }
            if (token.IsCancellationRequested)
            {
                throw new TransbankException($"Read operation Timeout");
            }
            CurrentResponse = Instance.Port.ReadExisting();
            Instance.Port.Write("");
            Console.WriteLine(CurrentResponse);
        }

        private bool ReadAck()
        {
            int intValue = Port.ReadByte();
            byte[] result = BitConverter.GetBytes(intValue);
            Array.Reverse(result);
            return Encoding.ASCII.GetString(result).Equals("");
        }

        private bool CantWrite() => Port == null || !Port.IsOpen;

        private string MessageWithLRC(string message)
        {
            int lrc = 0;
            for (int i = 1; i < message.Length; i++)
            {
                lrc ^= Encoding.ASCII.GetBytes(message.Substring(i, 1))[0];
            }
            Console.WriteLine("LRC Result : " + (char)lrc + " int: " + lrc);
            return message + (char)lrc;
        }
    }
}
