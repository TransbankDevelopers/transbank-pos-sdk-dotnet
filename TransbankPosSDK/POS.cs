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
        private static readonly byte ACK = 0x06;
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
                Port.ReadTimeout = _defaultTimeout;
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

        public SaleResponse Sale(int amount, string ticket, bool sendStatus = false)
        {
            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string message = $"0200|{amount}|{ticket}|||{Convert.ToInt32(sendStatus)}|";
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

        public MultiCodeSaleResponse MultiCodeSale(int amount, string ticket, long commerceCode = 0, bool sendStatus = false)
        {
            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string code = commerceCode != 0 ? commerceCode.ToString() : "";
            string message = $"0270|{amount}|{ticket}|| |{Convert.ToInt32(sendStatus)}|{code}|";
            try
            {
                WriteData(MessageWithLRC(message), intermediateMessages: sendStatus).Wait();
                return new MultiCodeSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeSaleException($"Unable to execute multicode sale on pos", e);
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

        public MultiCodeLastSaleResponse MultiCodeLastSale(bool getVoucherInfo)
        {
            try
            {
                string message = $"0280|{Convert.ToInt32(getVoucherInfo)}";
                WriteData(MessageWithLRC(message)).Wait();
                return new MultiCodeLastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeLastSaleException($"Unable to recover last sale from pos", e);
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

        public List<DetailResponse> Details(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<DetailResponse> details = new List<DetailResponse>();
            try
            {
                WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true).Wait();

                foreach (string sale in SaleDetail)
                {
                    details.Add(new DetailResponse(sale));
                }
            }
            catch (Exception e)
            {
                throw new TransbankSalesDetailException("Unabel to request sale detail on pos", e);
            }
            return details;
        }

        public List<MultiCodeDetailResponse> MultiCodeDetails(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<MultiCodeDetailResponse> details = new List<MultiCodeDetailResponse>();
            try
            {
                WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true).Wait();

                foreach (string sale in SaleDetail)
                {
                    details.Add(new MultiCodeDetailResponse(sale));
                }
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeDetailException("Unabel to request sale detail on pos", e);
            }
            return details;
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
                byte[] buffer = new byte[1];
                Port.Write("0100");
                Instance.Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
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
                byte[] buffer = new byte[1];
                Port.Write("0300\0");
                Instance.Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port {Port.PortName}", e);
            }
        }

        private async Task WriteData(string payload, bool intermediateMessages = false, bool saleDetail = false, bool printOnPOS = true)
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            CurrentResponse = "";
            if (Instance.CantWrite())
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
            }
            Port.Write(payload);
            bool ack = ReadAck(new CancellationTokenSource(_defaultTimeout).Token);
            
            if (ack)
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
                        if (!printOnPOS)
                        {
                            await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
                            string authorizationCode;
                            try
                            {
                                authorizationCode = CurrentResponse.Substring(1).Split('|')[5];
                                if (authorizationCode != "" && authorizationCode != " ")
                                {
                                    SaleDetail.Add(string.Copy(CurrentResponse));
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                authorizationCode = null;
                            }

                            while (authorizationCode != null && authorizationCode != "" && authorizationCode != " ")
                            {
                                await ReadMessage(new CancellationTokenSource(_defaultTimeout).Token);
                                try
                                {
                                    authorizationCode = CurrentResponse.Substring(1).Split('|')[5];
                                    if (authorizationCode != "" && authorizationCode != " ")
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task ReadMessage(CancellationToken token)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

        private bool ReadAck(CancellationToken token)
        {
            while (!token.IsCancellationRequested && Port.BytesToRead <= 0)
            {
            }
            if (token.IsCancellationRequested)
            {
                throw new TransbankException($"Read operation Timeout");
            }
            byte[] result = new byte[1];
            Instance.Port.BaseStream.ReadAsync(result, 0, 1, token).Wait();
            return result[0] == ACK;
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
