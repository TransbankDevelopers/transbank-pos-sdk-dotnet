using System.IO.Ports;
using System.Collections.Generic;

namespace Transbank.POS.Utils
{
    public static class Serial
    {
        public static List<string> ListPorts() => new List<string>(collection: SerialPort.GetPortNames());
    }
}