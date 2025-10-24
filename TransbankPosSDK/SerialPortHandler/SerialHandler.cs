using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Transbank.Exceptions.CommonExceptions;

namespace Transbank.SerialPortHandler
{
    public class SerialHandler : ISerialHandler
    {
        private SerialPort _port;

        public event Action<string> DataReceived;
        public bool IsOpen => _port.IsOpen;

        public List<string> ListPorts() => new List<string>(collection: SerialPort.GetPortNames());

        public void Open(string portName, int baudrate = 115200)
        {
            _port = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _port.DataReceived += OnPortDataReceived;
            _port.Open();
        }

        public void ClosePort()
        {
            if (!_port.IsOpen) return;
            try
            {
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                _port.Close();
            }
            catch (Exception e)
            {
                throw new TransbankException("Could not Close Serial Port: " + _port.PortName, e);
            }
        }

        public void Write(string data)
        {
            if (CantWrite())
            {
                throw new TransbankException("Unable to send message to port. Port is not open.");
            }

            Console.WriteLine($"Enviado al POS: {data}");
            Console.WriteLine($"[HEX] Enviado al POS: {ToHexString(data)}");
            _port.Write(data);
        }

        private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _port.ReadExisting();

                if (!string.IsNullOrEmpty(data))
                {
                    Console.WriteLine($"Datos recibidos: {data}");
                    Console.WriteLine($"[HEX] Datos recibidos: {ToHexString(data)}");
                    DataReceived?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data from port: {ex.Message}");
            }
        }

        protected bool CantWrite() => _port == null || !_port.IsOpen;
        
        protected string ToHexString(string text)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(text)).Replace('-', ' ');
        }
    }
}