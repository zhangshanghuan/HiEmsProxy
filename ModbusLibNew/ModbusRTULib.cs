using FluentModbus;
using System.IO.Ports;
using thinger.DataConvertLib;

namespace ModbusLibNew
{
    public class ModbusRTULib
    {
        object LOCK = new object();
        public SerialPort _SerialPort=null;
        public ModbusRtuClient? _RTUClient = null;
        public string _Data = string.Empty;
        public byte[] _DataByteArray = null;
        public string COM = string.Empty;
        baselib _baselib = new baselib();
        int ReadTimeout = 1000;
        int WriteTimeout = 1000;
        public ModbusEndianness _ModbusEndianness;//大端模式
        public bool Init(SerialPort SerialPort, int ReadTimeout = 1000, int WriteTimeout = 1000, ModbusEndianness _ModbusEndianness = ModbusEndianness.BigEndian)
        {
            try
            {
                this._ModbusEndianness = _ModbusEndianness;
                this.ReadTimeout = ReadTimeout;
                this.WriteTimeout = WriteTimeout;
                _SerialPort = SerialPort;
                 COM = SerialPort.PortName;
                _RTUClient = new ModbusRtuClient();
                _RTUClient.BaudRate = SerialPort.BaudRate;
                _RTUClient.Parity = SerialPort.Parity;
                _RTUClient.StopBits = SerialPort.StopBits;
                _RTUClient.Connect(COM, _ModbusEndianness);
                _RTUClient.ReadTimeout = ReadTimeout;
                _RTUClient.WriteTimeout = WriteTimeout;
                return _RTUClient.IsConnected;
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
                if (_RTUClient!=null)
                {
                    _RTUClient.BaudRate = _SerialPort.BaudRate;
                    _RTUClient.Parity = _SerialPort.Parity;
                    _RTUClient.StopBits = _SerialPort.StopBits;
                    _RTUClient.Connect(COM, _ModbusEndianness);
                    _RTUClient.ReadTimeout = ReadTimeout;
                    _RTUClient.WriteTimeout = WriteTimeout;
                    return _RTUClient.IsConnected;
                }                    
            }
            catch (Exception EX)
            {
              
            }
            return false;
        }

       

        public bool ReadAndWrite(ushort slaveAddress, ushort startAddress, ushort numberOfPoints, FunctionEnum functionCode, string Writedata = "")
        {
            lock (LOCK)
            {
                _Data = string.Empty;
                return ExecuteFunction((Byte)slaveAddress, startAddress, numberOfPoints, functionCode, Writedata);
            }
        }
      
        private bool ExecuteFunction(byte slaveAddress, ushort startAddress, ushort numberOfPoints, FunctionEnum functionCode, string Writedata)
        {
            try
            {
                _Data = string.Empty;
                coilDatauffer = null;
                registerBuffer = null;
                bool[] resbool = null;
                Span<Byte> ByteData=null;              
                Span<ushort> ushortData;
                _DataByteArray = null;
                var sleepTime = TimeSpan.FromMilliseconds(100);
                if (functionCode != null && _RTUClient != null && _RTUClient.IsConnected)
                {                   
                    switch (functionCode)
                    {
                        case FunctionEnum.ReadCoils://读取单个线圈    ReadCoils = 0x01,                                         
                            ByteData = _RTUClient.ReadCoils(slaveAddress, startAddress, numberOfPoints);
                            //   resbool = BitLib.GetBitArrayFromByteArray(ByteData.ToArray(), numberOfPoints);
                            // _Data = string.Join(" ", resbool.ToArray());
                            break;
                        case FunctionEnum.ReadDiscreteInputs://读取输入线圈/离散量线圈  ReadDiscreteInputs = 0x02                      
                            ByteData = _RTUClient.ReadDiscreteInputs(slaveAddress, startAddress, numberOfPoints);
                            //   resbool = BitLib.GetBitArrayFromByteArray(ByteData.ToArray(), numberOfPoints);
                            //  _Data = string.Join(" ", resbool.ToArray());
                            break;
                        case FunctionEnum.ReadHoldingRegisters://读取保持寄存器     ReadHoldingRegisters = 0x03                       
                            ByteData = _RTUClient.ReadHoldingRegisters<byte>(slaveAddress, startAddress, numberOfPoints * 2);
                            //_Data = _baselib.DataConversion(UShortLib.GetUShortArrayFromByteArray(ByteData.ToArray()), datatype);
                            break;
                        case FunctionEnum.ReadInputRegisters://读取输入寄存器  ReadInputRegisters = 0x04                         
                            ByteData = _RTUClient.ReadInputRegisters<byte>(slaveAddress, startAddress, numberOfPoints * 2);
                            //  _Data = _baselib.DataConversion(UShortLib.GetUShortArrayFromByteArray(ByteData.ToArray()), datatype);
                            break;
                        case FunctionEnum.WriteSingleRegister://写单个保持寄存器 WriteSingleRegister = 0x06
                            SetWriteParametes(slaveAddress, startAddress, Writedata, false);
                            if (registerBuffer == null || registerBuffer.Length <= 0) return false;
                            _RTUClient.WriteSingleRegister(slaveAddress, startAddress, registerBuffer[0]);
                            break;
                        case FunctionEnum.WriteMultipleRegisters://写一组保持寄存器 WriteMultipleRegisters = 0x10
                            SetWriteParametes(slaveAddress, startAddress, Writedata, false);
                            if (registerBuffer == null || registerBuffer.Length <= 0) return false;
                            _RTUClient.WriteMultipleRegisters(slaveAddress, startAddress, registerBuffer);
                            break;
                        case FunctionEnum.WriteSingleCoil://写单个线圈 WriteSingleCoil = 0x05
                            SetWriteParametes(slaveAddress, startAddress, Writedata, true);
                            if (coilDatauffer == null || coilDatauffer.Length <= 0) return false;
                            _RTUClient.WriteSingleCoil(slaveAddress, startAddress, coilDatauffer[0]);
                            break;
                        case FunctionEnum.WriteMultipleCoil://写一组线圈 WriteMultipleCoil 0x0F
                            SetWriteParametes(slaveAddress, startAddress, Writedata, true);
                            if (coilDatauffer == null || coilDatauffer.Length <= 0) return false;
                            foreach (var item in coilDatauffer)
                            {
                                _RTUClient.WriteSingleCoil(slaveAddress, startAddress, item);
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
                _RTUClient?.Close();                
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
                        if (strarr[i] == "False")
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
