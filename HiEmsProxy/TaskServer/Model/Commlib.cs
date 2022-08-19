using System;
using System.Collections.Generic;
using System.IO.Ports;
using HiEmsProxy.TaskServer.Actuator;

namespace HiEmsProxy.TaskServer.Model
{
    public static class Commlib
    {
        //定义通讯执行器集合
        public static List<ModbusTcpExecute> _ListTcpModbusLib = new List<ModbusTcpExecute>();
        public static List<ModbusRtuExecute> _ListRtuModbusLib = new List<ModbusRtuExecute>();
        public static List<SerialExecute> _ListSerialLib = new List<SerialExecute>();
        //定义一个串口通讯集合     
        public static SerialExecute CreatSerial(string COM, int BaudRate, int DataBits, Parity Parity, StopBits StopBits,int ID)
        {          
                try
                {
                    foreach (SerialExecute item in _ListSerialLib)
                    {
                        if (COM == item._SerialLib._SerialPort.PortName)
                        {
                            if (BaudRate == item._SerialLib._SerialPort.BaudRate
                            && DataBits == item._SerialLib._SerialPort.DataBits
                            && Parity == item._SerialLib._SerialPort.Parity
                            && StopBits == item._SerialLib._SerialPort.StopBits )
                            {
                                return item;
                            }
                        }
                    }
                    SerialPort _SerialPort = new SerialPort()
                    {
                        PortName = COM,
                        BaudRate = BaudRate,
                        DataBits = DataBits,
                        Parity = Parity,
                        StopBits = StopBits
                    };            
                    SerialExecute _SerialExcute = new SerialExecute(_SerialPort, ID);
                    _ListSerialLib.Add(_SerialExcute);
                    return _SerialExcute;
                }
                catch (Exception ex)
                {
                    return null;
                }            
        }

        //定义一个CAN通讯集合

        //创建一个modbus tcp 执行器对象     
        public static ModbusTcpExecute CreatModbusTcp(string Ip, int port,int ID)
        {
          
                try
                {
                    foreach (ModbusTcpExecute item in _ListTcpModbusLib)
                    {
                        if (item._ModbusLib.IP == Ip && item._ModbusLib.PORT == port)
                        {
                            return item;                           
                        }                                       
                    }
                    ModbusTcpExecute _ModbusTcpExcute = new ModbusTcpExecute(Ip, port,ID);
                    _ListTcpModbusLib.Add(_ModbusTcpExcute);
                    return _ModbusTcpExcute;
                }
                catch (Exception ex)
                {
                    return null;
                }           
        }

        //创建一个modbus RTU 执行器 对象       
        public static ModbusRtuExecute CreatModbusRtu(string COM, int BaudRate, int DataBits, Parity Parity, StopBits StopBits,int ID)
        {
            try
            {
                foreach (ModbusRtuExecute item in _ListRtuModbusLib)
                {
                    if (COM == item._ModbusLib._SerialPort.PortName)
                    {
                        if (BaudRate == item._ModbusLib._SerialPort.BaudRate
                        && DataBits == item._ModbusLib._SerialPort.DataBits
                        && Parity == item._ModbusLib._SerialPort.Parity
                        && StopBits == item._ModbusLib._SerialPort.StopBits)
                        {
                            return item;
                        }
                    }
                }
                SerialPort _SerialPort = new SerialPort()
                {
                    PortName = COM,
                    BaudRate = BaudRate,
                    DataBits = DataBits,
                    Parity = Parity,
                    StopBits = StopBits
                };
                ModbusRtuExecute _ModbusRtuExcute = new ModbusRtuExecute(_SerialPort, ID);
                _ListRtuModbusLib.Add(_ModbusRtuExcute);
                return _ModbusRtuExcute;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
