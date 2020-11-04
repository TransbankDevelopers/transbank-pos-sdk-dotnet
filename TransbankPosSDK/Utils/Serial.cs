using System.Collections.Generic;
using System.IO.Ports;

namespace Transbank.POS.Utils
{
    public static class Serial
    {
        public static List<string> ListPorts() => new List<string>(collection: SerialPort.GetPortNames());
    }
}