using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transbank.POS.Exceptions;
using Transbank.POS.Responses;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly POS _instance = new POS();
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
        private List<string> SaleDetail;

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

        public LastSaleResponse LastSale()
        {
            try
            {
                WriteData("0250|x").Wait();
                return new LastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLastSaleException($"Unable to recover last sale from pos", e);
            }
        }

        public RefundResponse Refund(int operationID)
        {
            string message = $"1200|{operationID}|";

            try
            {
                WriteData(MessageWithLRC(message)).Wait();
                return new RefundResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankRefundException("Unable to make Refund on POS", e);
            }
        }

        public TotalsResponse Totals()
        {
            try
            {
                WriteData("0700||").Wait();
                return new TotalsResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankTotalsException("Unable to get totals from POS", e);
            }
        }

        public List<DetailResponse> Details(bool sendMessageToRegister = false)
        {
            string message = $"0260|{Convert.ToInt32(sendMessageToRegister)}|";
            try
            {
                var details = new List<DetailResponse>();
                WriteData(MessageWithLRC(message), sendMessageToRegister: sendMessageToRegister, saleDetail: true).Wait();

                foreach (string sale in SaleDetail)
                {
                    details.Add(new DetailResponse(sale));
                }

                return details;
            }
            catch (Exception e)
            {
                throw new TransbankSalesDetailException("Unabel to request sale detail on pos", e);
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
                if (saleDetail)
                {
                    SaleDetail = new List<string>();
                    if (sendMessageToRegister)
                    {
                        await ReadMessage(new CancellationTokenSource(90000).Token);
                        string authorizationCode = "";
                        try
                        {
                            authorizationCode = CurrentResponse.Substring(1).Split('|')[5];
                            if (authorizationCode != "")
                            {
                                SaleDetail.Add(string.Copy(CurrentResponse));
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            authorizationCode = null;
                        }

                        while (authorizationCode != null && authorizationCode != "")
                        {
                            await ReadMessage(new CancellationTokenSource(90000).Token);
                            try
                            {
                                authorizationCode = CurrentResponse.Substring(1).Split('|')[5];
                                if (authorizationCode != "")
                                {
                                    SaleDetail.Add(string.Copy(CurrentResponse));
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                authorizationCode = null;
                            }
                        }
                    }
                }
                else
                {
                    await ReadMessage(new CancellationTokenSource(90000).Token);
                }
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
