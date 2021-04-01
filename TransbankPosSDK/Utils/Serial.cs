using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System;
using Transbank.POS.CommonExceptions;
using System.Threading.Tasks;
using System.Threading;
using Transbank.POS.CommonResponses;

namespace Transbank.POS.Utils
{
    public class Serial
    {
        protected static readonly byte ACK = 0x06;
        protected static readonly int DEFAULT_TIMEOUT = 150000;

        protected string _currentResponse;
        protected List<string> SaleDetail;
        private int _timeout;
        public event EventHandler<IntermediateResponse> IntermediateResponseChange;

        protected static SerialPort Port { get; private set; }

        public List<string> ListPorts() => new List<string>(collection: SerialPort.GetPortNames());

        private int ReadTimeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                Port.ReadTimeout = _timeout;
            }
        }

        protected string CurrentResponse
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

        protected void DiscardBuffer()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
        }

        public void OpenPort(string portName, int baudrate = 115200)
        {
            if (Port != null && Port.IsOpen) ClosePort();
            try
            {
                Port = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);
                Port.Open();
                ReadTimeout = DEFAULT_TIMEOUT;
                DiscardBuffer();
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
                DiscardBuffer();
                Port.Close();
            }
            catch (Exception e)
            {
                throw new TransbankException("Could not Close Serial Port: " + Port.PortName, e);
            }
        }

        public bool IsPortOpen => Port.IsOpen;

        protected bool CantWrite() => Port == null || !Port.IsOpen;

        protected async Task WriteData(string payload, bool intermediateMessages = false, bool saleDetail = false, bool printOnPOS = true)
        {
            DiscardBuffer();

            CurrentResponse = "";
            if (CantWrite())
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
            }

            Console.WriteLine($"Out: {ToHexString(payload)}");

            Port.Write(payload);
            bool ack = ReadAck(new CancellationTokenSource(_timeout).Token);

            if (ack)
            {
                Console.WriteLine("Read ACK OK");
                if (intermediateMessages)
                {
                    await ReadMessage(new CancellationTokenSource(_timeout).Token);
                    string responseCode = CurrentResponse.Substring(1).Split('|')[1];
                    while (responseCode == "78" || responseCode == "79" ||
                        responseCode == "80" || responseCode == "81" || responseCode == "82")
                    {
                        await ReadMessage(new CancellationTokenSource(_timeout).Token);
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
                            await ReadMessage(new CancellationTokenSource(_timeout).Token);
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
                                await ReadMessage(new CancellationTokenSource(_timeout).Token);
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
                        await ReadMessage(new CancellationTokenSource(_timeout).Token);
                    }
                }
            }
            else
            {
                throw new TransbankException($"Unable to send message to {Port.PortName}");
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async Task ReadMessage(CancellationToken token)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            while (!token.IsCancellationRequested && Port.BytesToRead <= 0)
            {
            }
            if (token.IsCancellationRequested)
            {
                throw new TransbankException($"Read operation Timeout");
            }

            Port.ReadTo("");
            CurrentResponse = "" + Port.ReadTo("");
            Port.Write("");
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
            Port.BaseStream.ReadAsync(result, 0, 1, token).Wait();
            return CheckACK(result[0]);
        }

        protected string MessageWithLRC(string message)
        {
            return message + Lrc(message);
        }

        protected char Lrc(string message)
        {
            int lrc = 0;
            for (int i = 1; i < message.Length; i++)
            {
                lrc ^= Encoding.ASCII.GetBytes(message.Substring(i, 1))[0];
            }
            return (char)lrc;
        }

        protected bool CheckACK(byte ack)
        {
            Console.WriteLine($"In: {string.Format("0:X2", ack)}");
            return ack == ACK;
        }

        private string ToHexString(string text)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(text)).Replace('-', ' ');
        }
    }
}