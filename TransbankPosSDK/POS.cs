using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System.Collections.Generic;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly int _defaultTimeout = 150000;
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

        public bool IsPortOpen => Port.IsOpen;

        public SerialPort Port { get; private set; }
        

        private POS()
        {
        }

        public static POS Instance { get; } = new POS();

        public void OpenPort(string portName, int baudrate = 115200)
        {
            if (Port != null && Port.IsOpen) ClosePort();
            try
            {
                Port = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);
                Port.Open();
                Port.ReadTimeout = 15000;
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
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to Poll port {Port.PortName} is closed");
            }
            try
            {
                Port.Write("0100");
                //Thread.Sleep(500);
                string response = ((char)Port.ReadByte()).ToString();
                return response.Equals("");
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public bool SetNormalMode()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to Set Normal Mode port {Port.PortName} is closed");
            }
            try
            {
                Port.Write("0300\0");
                //Thread.Sleep(500);
                string response = ((char)Port.ReadByte()).ToString();
                if (response.Equals(""))
                {
                    ClosePort();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port {Port.PortName}", e);
            }
        }

        private async Task WriteData(string payload, bool intermediateMessages = false, bool saleDetail = false, bool sendMessageToRegister = false)
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            CurrentResponse = "";
            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
            }
            Port.Write(payload);
            Task<bool> ack = ReadAck(new CancellationTokenSource(2000).Token);
            _ = await ack;
            if (ack.Result)
            {
                Console.WriteLine("Read ACK OK");
                if (intermediateMessages)
                {
                    await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
                    string responseCode = CurrentResponse.Substring(1).Split('|')[1];
                    while (responseCode == "78" || responseCode == "79" ||
                        responseCode == "80" || responseCode == "81" || responseCode == "82")
                    {
                        await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
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
                            await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
                            string authorizationCode;
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
                                await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
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
                        await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
                    }
                }
            }
            else
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
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

            Instance.Port.ReadTo("");
            CurrentResponse = "" + Instance.Port.ReadTo("");
            Instance.Port.Write("");
            Console.WriteLine(CurrentResponse);
        }

        private async Task<bool> ReadAck(CancellationToken token)
        {
            while (!token.IsCancellationRequested && Port.BytesToRead <= 0)
            {
            }
            if (token.IsCancellationRequested)
            {
                throw new TransbankException($"Read operation Timeout");
            }
            byte[] result = new byte[1];
            _ = await Instance.Port.BaseStream.ReadAsync(result, 0, 1, token);
            return Encoding.ASCII.GetString(result).Equals("");
        }

        private bool CantWrite() => Port == null || !Port.IsOpen;

        private string MessageWithLRC(string message)
        {
            return message + Lrc(message);
        }

        private char Lrc(string message)
        {
            int lrc = 0;
            for (int i = 1; i < message.Length; i++)
            {
                lrc ^= Encoding.ASCII.GetBytes(message.Substring(i, 1))[0];
            }
            return (char)lrc;
        }
    }
}
