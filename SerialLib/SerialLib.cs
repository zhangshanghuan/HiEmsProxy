using System.IO.Ports;
using System.Text;
namespace Serial_Lib
{  
    public class SerialLib
    {
        public SerialPort? _SerialPort;
        public StringBuilder sb = new StringBuilder();
        private List<byte> buffer = new List<byte>(4096);
        AutoResetEvent _AutoResetEvent = new AutoResetEvent(false);
        public bool IsConnected
        {
            get
            {
                return _SerialPort != null ? _SerialPort.IsOpen : false;
            }
        }
        public bool Init(SerialPort _SerialPort)
        {
            try
            {
                this._SerialPort = _SerialPort;
                _SerialPort.DataReceived += _SerialPort_DataReceived;
                _SerialPort.Open();
                return _SerialPort.IsOpen;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return false;
        }
        public bool ReConnect()
        {
            try
            {
                if (_SerialPort != null)
                {
                    _SerialPort.Open();
                    return _SerialPort.IsOpen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
         
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_SerialPort != null)
                {
                    int length = _SerialPort.BytesToRead;
                    if (length > 0)
                    {
                        byte[] result = new byte[length];
                        //读取串口缓冲区的字节数据
                        _SerialPort.Read(result, 0, _SerialPort.BytesToRead);
                        buffer.AddRange(result);
                        string dd = Encoding.ASCII.GetString(result);
                        Console.WriteLine("串口接收数据：" + dd);
                        sb.Append(dd);
                        if (sb.ToString().Contains("\r\n"))
                        {
                            _AutoResetEvent.Set();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _SerialPort?.Dispose();
            }
        }

        public  string  GetData(string cmd)
        {
            sb.Clear();
            _AutoResetEvent.Reset();
            SendDataMethod(cmd);
            if (_AutoResetEvent.WaitOne(2000))
            {
                return sb.ToString().Replace("\r\n", "");
            }
            return "";
        }


        /// <summary>
        /// 打开串口
        /// </summary>
        public bool Open()
        {
            try
            {
                if (_SerialPort == null) return false;
                //打开串口
                _SerialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
         
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public bool Close()
        {
            try
            {
                if (_SerialPort == null) return false;
                _SerialPort.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }         
        }

        public void SendDataMethod(byte[] data)
        {
            buffer.Clear();
            sb.Clear();
            if (_SerialPort == null) return;        
            //获取串口状态，true为已打开，false为未打开
            bool isOpen = _SerialPort.IsOpen;
            if (!isOpen) Open();      
            //发送字节数组
            //参数1：包含要写入端口的数据的字节数组。
            //参数2：参数中从零开始的字节偏移量，从此处开始将字节复制到端口。
            //参数3：要写入的字节数。 
            _SerialPort.Write(data, 0, data.Length);
        }

        public void SendDataMethod(string data)
        {
            buffer.Clear();
            sb.Clear();
            if (_SerialPort == null) return;
            //获取串口状态，true为已打开，false为未打开
            bool isOpen = _SerialPort.IsOpen;
            if (!isOpen) Open();         
            //直接发送字符串
            _SerialPort.Write(data);
        }       
    }
}