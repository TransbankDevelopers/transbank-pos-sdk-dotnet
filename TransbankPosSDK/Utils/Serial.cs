using System.Collections.Generic;

namespace Transbank.POS.Utils
{
    public static class Serial
    {
        public static List<string> ListPorts()
        {
            return new List<string>(TransbankWrap.list_ports().Split('|'));
        }
    }
}
