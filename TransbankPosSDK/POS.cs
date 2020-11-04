using System;
using System.IO.Ports;
using Transbank.POS.Utils;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System.Collections.Generic;
using System.Text;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly POS _instance = new POS();
        private readonly int _defaultBaudrate = 115200;
        private string _currentResponse;
        private string CurrentResponse
        {
            get { return _currentResponse; }
            set
            {
                _currentResponse = value;
            }
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
        public SaleResponse Sale(int amount, string ticket)
        {
            if (amount <= 0)
            {
                throw new TransbankException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankException("Ticket must be 6 characters.");
            }
            if (_configured)
            {
                try
                {
                    SaleResponse response = new SaleResponse(TransbankWrap.sale(amount, ticket, false));
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankSaleException("Sale returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankSaleException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute Sale on pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
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

        public bool SetNormalMode()
        {
            if (_configured)
            {
                try
                {
                    return TransbankWrap.set_normal_mode() == TbkReturn.TBK_OK;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to set normal mode", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public bool Poll()
        {
            if (_configured)
            {
                try
                {
                    return TransbankWrap.poll() == TbkReturn.TBK_OK;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to locate POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public CloseResponse Close()
        {
            if (_configured)
            {
                try
                {
                    CloseResponse response = new CloseResponse(TransbankWrap.close());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankCloseException("Close retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankCloseException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute close in pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public LoadKeysResponse LoadKeys()
        {
            if (_configured)
            {
                try
                {
                    LoadKeysResponse response = new LoadKeysResponse(TransbankWrap.load_keys());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankLoadKeysException("Load Keys retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankLoadKeysException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to load Keys in pos", e);
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
