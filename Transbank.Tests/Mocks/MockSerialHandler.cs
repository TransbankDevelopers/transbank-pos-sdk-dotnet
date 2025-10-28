using System;
using System.Collections.Generic;
using Transbank.SerialPortHandler;

namespace Transbank.Tests.Mocks
{
    public class MockSerialHandler : ISerialHandler
    {
        public event Action<string>? DataReceived;
        public bool IsOpen { get; private set; } = true;
        public List<string> WrittenData { get; } = new();

        public void Write(string data)
        {
            WrittenData.Add(data);
        }

        public void Open(string portName, int baudrate = 115200)
        {
            IsOpen = true;
        }

        public void ClosePort()
        {
            IsOpen = false;
        }

        public List<string> ListPorts() => new() { "FAKE_PORT" };

        public void SimulateIncoming(string message)
        {
            DataReceived?.Invoke(message);
        }

        public void ClearSubscribers()
        {
            DataReceived = null;
        }
    }
}
