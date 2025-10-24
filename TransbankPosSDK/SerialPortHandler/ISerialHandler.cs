using System;
using System.Collections.Generic;

namespace Transbank.SerialPortHandler
{
    public interface ISerialHandler
    {
        event Action<string> DataReceived;

        bool IsOpen { get; }

        List<string> ListPorts();

        void Open(string portName, int baudrate = 115200);

        void ClosePort();

        void Write(string data);
    }
}