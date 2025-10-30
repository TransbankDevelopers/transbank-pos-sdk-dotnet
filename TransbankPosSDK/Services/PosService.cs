using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Transbank.SerialPortHandler;
using System.Linq;

namespace Transbank.Services
{
    public class PosService
    {
        private readonly ISerialHandler _handler;
        private readonly StringBuilder _buffer = new StringBuilder();
        private const string INTERMEDIATE_MSG_CODE = "0900";
        private const int AUTOSERVICIO_START_INDEX = 0;
        private const int INTEGRADO_START_INDEX = 1;
        protected static readonly int LRC_LENGTH = 1;
        public static readonly char ACK = (char)0x06;
        protected static readonly char NACK = (char)0x15;
        protected static readonly char STX = (char)0x02;
        protected static readonly char ETX = (char)0x03;
        public event EventHandler<string> IntermediateResponseReceived;
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(150);
        public enum Model
        {
            AUTOSERVICIO = 0,
            INTEGRADO = 1,
        }
        protected Model _posModel { get; private set; }
        public PosService(ISerialHandler handler, Model posModel = Model.INTEGRADO)
        {
            _handler = handler;
            _posModel = posModel;
        }

        protected virtual void OnIntermediateResponseReceived(string response)
        {
            IntermediateResponseReceived?.Invoke(this, response);
        }
        public async Task<string> ProcessNormalCommand(string message, bool shortResponse = false)
        {
            string fullMessage = CreateFullMessage(message);
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            void ResponseHandler(string data) => HandleCommonResponse(data, tcs, shortResponse);
            _handler.DataReceived += ResponseHandler;

            try
            {
                return await SendCommand(tcs, fullMessage);
            }
            finally
            {
                _handler.DataReceived -= ResponseHandler;
            }
        }

        public async Task<List<string>> ProcessDetailsCommand(string message, bool printOnPOS)
        {
            string fullMessage = CreateFullMessage(message);
            var tcs = new TaskCompletionSource<List<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var responseList = new List<string>();
            void ResponseHandler(string data) => HandleDetailsResponse(data, tcs, responseList, printOnPOS);
            _handler.DataReceived += ResponseHandler;

            try
            {
                return await SendCommand(tcs, fullMessage);
            }
            finally
            {
                _handler.DataReceived -= ResponseHandler;
            }
        }

        private async Task<T> SendCommand<T>( TaskCompletionSource<T> tcs, string fullMessage)
        {
            _buffer.Clear();
            _handler.Write(fullMessage);

            using (var cts = new CancellationTokenSource(_defaultTimeout))
            {
                cts.Token.Register(() =>
                {
                    if (!tcs.Task.IsCompleted)
                        tcs.TrySetCanceled();
                });

                return await tcs.Task;
            }
        }

        private void HandleCommonResponse(string rawData, TaskCompletionSource<string> tcs, bool shortResponse)
        {
            if (rawData == ACK.ToString())
            {
                if (shortResponse)
                {
                    tcs.TrySetResult(ACK.ToString());
                }
                _buffer.Clear();
                return;
            }

            _buffer.Append(rawData);
            if (IsMessageCompleted(_buffer.ToString()))
            {
                if (!CheckReceivedLRC(_buffer.ToString()))
                {
                    SendNack();
                    _buffer.Clear();
                    return;
                }

                if (IsIntermediateMessage(_buffer.ToString()))
                {
                    OnIntermediateResponseReceived(_buffer.ToString());
                    _buffer.Clear();
                    return;
                }
                SendAck();
                string payload = ExtractPayload(_buffer.ToString());
                tcs.TrySetResult(payload);
                _buffer.Clear();
            }
        }

        private void HandleDetailsResponse(string rawData, TaskCompletionSource<List<string>> tcs, List<string> responseList, bool printOnPOS)
        {
            if (rawData == ACK.ToString())
            {
                if (printOnPOS)
                {
                    tcs.TrySetResult(responseList);
                }
                _buffer.Clear();
                return;
            }
            _buffer.Append(rawData);
            if (IsMessageCompleted(_buffer.ToString()))
            {
                if (!CheckReceivedLRC(_buffer.ToString()))
                {
                    SendNack();
                    _buffer.Clear();
                    return;
                }
                string payload = ExtractPayload(_buffer.ToString());
                responseList.Add(payload);
                SendAck();
                _buffer.Clear();
                if (IsDetailsListCompleted(responseList))
                {
                    tcs.TrySetResult(responseList);
                    return;
                }
            }
        }

        private bool IsDetailsListCompleted(List<string> responses)
        {
            int emptyCount = 0;
            foreach (var f in responses.AsEnumerable().Reverse())
            {
                var parts = f.Split('|');
                var auth = parts.Length > 5 ? parts[5].Trim() : string.Empty;

                if (string.IsNullOrEmpty(auth))
                    emptyCount++;
                else
                    break;
            }

            return emptyCount >= 2;
        }

        public List<string> getPorts()
        {
            return _handler.ListPorts();
        }

        public void OpenPort(string portName, int baudrate = 115200)
        {
            _handler.Open(portName, baudrate);
        }

        public void ClosePort()
        {
            _handler.ClosePort();
        }
        public bool IsPortOpen => _handler.IsOpen;

        private bool IsMessageCompleted(string data)
        {
            int etxIndex = data.IndexOf(ETX);
            return etxIndex != -1 && etxIndex + 1 < data.Length;
        }

        private bool IsIntermediateMessage(string response)
        {
            return response.Length >= 1 && response.Split('|')[0].Contains(INTERMEDIATE_MSG_CODE);
        }

        protected char CalculateLrc(string message)
        {
            char lrc = (char)0;
            for (int i = 0; i < message.Length; i++)
            {
                lrc ^= message[i];
            }
            return lrc;
        }

        protected bool CheckReceivedLRC(string response)
        {
            if (response == String.Empty)
            {
                return false;
            }

            if (IsIntermediateMessage(response))
            {
                return true;
            }
            int lrcIndex = response.Length - 1;
            char ReceivedLrc = response[lrcIndex];
            char CalculatedLrc = CalculateResponseLrc(response);
            return ReceivedLrc == CalculatedLrc;
        }

        private char CalculateResponseLrc(string message)
        {
            int startIndex = _posModel == Model.AUTOSERVICIO ? AUTOSERVICIO_START_INDEX : INTEGRADO_START_INDEX;
            int charsToKeep = message.Length - startIndex - LRC_LENGTH;
            string trimmedMessage = message.Substring(startIndex, charsToKeep);
            return CalculateLrc(trimmedMessage);
        }

        protected string CreateFullMessage(string message)
        {
            return $"{STX}{message}{ETX}{CalculateLrc(message + ETX)}";
        }

        private void SendAck()
        {
            _handler.Write($"{ACK}");
        }

        private void SendNack()
        {
            _handler.Write($"{NACK}");
        }

        private string ExtractPayload(string message)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;

            int stxIndex = message.IndexOf(STX);
            int etxIndex = message.IndexOf(ETX);

            if (stxIndex == -1 || etxIndex == -1 || etxIndex <= stxIndex + 1)
                return string.Empty;
            return message.Substring(stxIndex + 1, etxIndex - stxIndex - 1);
        }

    }
}
