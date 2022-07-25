using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusLibNew
{
    public enum FunctionEnum
    {    
        ReadCoils =0x01,
        ReadDiscreteInputs=0x02,
        ReadHoldingRegisters = 0x03,
        ReadInputRegisters=0x04,
        WriteSingleRegister=0x06,
        WriteMultipleRegisters=0x10,
        WriteSingleCoil=0x05,
        WriteMultipleCoil=0x0F
    }  
}
