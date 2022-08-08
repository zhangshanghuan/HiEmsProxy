using FluentModbus;
using System.Net;
using thinger.DataConvertLib;
namespace ModbusLibNew
{
    public class ModbusTCPLib
    {
        public ModbusTcpClient? _TcpClient = null;
        object LOCK = new object();
        public string IP = string.Empty;
        public int PORT = 502;
        public string _Data = string.Empty;
        public byte[] _DataByteArray = null;
        baselib _baselib = new baselib();
        //大小端模式
        ModbusEndianness _ModbusEndianness;
        public bool Init(string ip, int port, int ReadTimeout = 1000, int WriteTimeout = 1000, ModbusEndianness _ModbusEndianness = ModbusEndianness.BigEndian)
        {
            try
            {
                 this. _ModbusEndianness = _ModbusEndianness;
                 IP = ip;
                 PORT = port;
                _TcpClient = new ModbusTcpClient();
                _TcpClient.Connect(IPAddress.Parse(ip), _ModbusEndianness);
                _TcpClient.ReadTimeout = ReadTimeout;
                _TcpClient.WriteTimeout = WriteTimeout;
                return _TcpClient.IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
        //重新连接
        public bool ReConnect()
        {
            try
            {
                if (_TcpClient != null) _TcpClient = null;
                _TcpClient = new ModbusTcpClient();
                _TcpClient.Connect(IPAddress.Parse(IP), _ModbusEndianness);
                _TcpClient.ReadTimeout = 1000;
                _TcpClient.WriteTimeout = 1000;
                return true;
            }
            catch (Exception EX)
            {
                _TcpClient = null;
            }
            return false;
        }

        public bool ReadAndWrite(ushort slaveAddress, ushort startAddress, ushort numberOfPoints, FunctionEnum functionCode, string Writedata ="")
        {
            lock (LOCK)
            {
                _Data = string.Empty;
                return ExecuteFunction((Byte)slaveAddress, startAddress, numberOfPoints, functionCode, Writedata);
            }
        }

        private bool ExecuteFunction(byte slaveAddress, ushort startAddress, ushort numberOfPoints, FunctionEnum functionCode,string Writedata)
        {
            try
            {
                _Data = string.Empty;
                _DataByteArray = null;
                coilDatauffer = null;
                registerBuffer = null;
                bool[] resbool = null;
                Span<Byte> ByteData=null;
                Span<ushort> ushortData;
                if (functionCode != null && _TcpClient != null && _TcpClient.IsConnected)
                {
                    switch (functionCode)
                    {
                        case FunctionEnum.ReadCoils://读取单个线圈    ReadCoils = 0x01,                                         
                            ByteData = _TcpClient.ReadCoils(slaveAddress, startAddress, numberOfPoints);
                         //   resbool = BitLib.GetBitArrayFromByteArray(ByteData.ToArray(), numberOfPoints);
                           // _Data = string.Join(" ", resbool.ToArray());
                            break;
                        case FunctionEnum.ReadDiscreteInputs://读取输入线圈/离散量线圈  ReadDiscreteInputs = 0x02                      
                            ByteData = _TcpClient.ReadDiscreteInputs(slaveAddress, startAddress, numberOfPoints);
                         //   resbool = BitLib.GetBitArrayFromByteArray(ByteData.ToArray(), numberOfPoints);
                          //  _Data = string.Join(" ", resbool.ToArray());
                            break;
                        case FunctionEnum.ReadHoldingRegisters://读取保持寄存器     ReadHoldingRegisters = 0x03                       
                            ByteData = _TcpClient.ReadHoldingRegisters<byte>(slaveAddress, startAddress, numberOfPoints*2);                        
                            //_Data = _baselib.DataConversion(UShortLib.GetUShortArrayFromByteArray(ByteData.ToArray()), datatype);
                            break;
                        case FunctionEnum.ReadInputRegisters://读取输入寄存器  ReadInputRegisters = 0x04                         
                            ByteData = _TcpClient.ReadInputRegisters<byte>(slaveAddress, startAddress, numberOfPoints*2);
                          //  _Data = _baselib.DataConversion(UShortLib.GetUShortArrayFromByteArray(ByteData.ToArray()), datatype);
                            break;
                        case FunctionEnum.WriteSingleRegister://写单个保持寄存器 WriteSingleRegister = 0x06
                            SetWriteParametes(slaveAddress, startAddress, Writedata, false);
                            if (registerBuffer == null || registerBuffer.Length <= 0) return false;
                            _TcpClient.WriteSingleRegister(slaveAddress, startAddress, registerBuffer[0]);
                            break;
                        case FunctionEnum.WriteMultipleRegisters://写一组保持寄存器 WriteMultipleRegisters = 0x10
                            SetWriteParametes(slaveAddress, startAddress, Writedata, false);
                            if (registerBuffer == null || registerBuffer.Length <= 0) return false;
                            _TcpClient.WriteMultipleRegisters(slaveAddress, startAddress, registerBuffer);
                            break;
                        case FunctionEnum.WriteSingleCoil://写单个线圈 WriteSingleCoil = 0x05
                            SetWriteParametes(slaveAddress, startAddress, Writedata, true);
                            if (coilDatauffer == null || coilDatauffer.Length <= 0) return false;
                            _TcpClient.WriteSingleCoil(slaveAddress, startAddress, coilDatauffer[0]);
                            break;
                        case FunctionEnum.WriteMultipleCoil://写一组线圈 WriteMultipleCoil 0x0F
                            SetWriteParametes(slaveAddress, startAddress, Writedata, true);
                            if (coilDatauffer == null || coilDatauffer.Length <= 0) return false;
                            foreach (var item in coilDatauffer)
                            {
                                _TcpClient.WriteSingleCoil(slaveAddress, startAddress, item);
                                startAddress++;
                            }
                            break;
                        default: return false;
                    }
                    _DataByteArray = ByteData.ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _TcpClient?.Disconnect();
            }
            return false;
        }

        public bool[]? coilDatauffer;
        public ushort[]? registerBuffer;
        private void SetWriteParametes(byte slaveAddress, ushort startAddress, string data, bool Is)
        {
            try
            {
                if (data == "") return;
                //判断是否写线圈
                if (Is)
                {
                    string[] strarr = data.Split(',');
                    coilDatauffer = new bool[strarr.Length];
                    //转化为bool数组
                    for (int i = 0; i < strarr.Length; i++)
                    {
                        if (strarr[i] == "0")
                        {
                            coilDatauffer[i] = false;
                        }
                        else
                        {
                            coilDatauffer[i] = true;
                        }
                    }
                }
                else
                {
                    //转化ushort数组
                    string[] strarr = data.Split(',');
                    registerBuffer = new ushort[strarr.Length];
                    for (int i = 0; i < strarr.Length; i++)
                    {
                        registerBuffer[i] = ushort.Parse(strarr[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
