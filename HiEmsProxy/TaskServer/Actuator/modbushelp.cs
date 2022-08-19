using ModbusLibNew;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using HiEmsProxy.TaskServer.Base;
using HiEMS.Model.Models;
using HiEMS.Model.Dto;

namespace HiEmsProxy.TaskServer.Actuator
{
    /// <summary>
    /// 地址排序从小到大
    /// </summary>
    public class AddressComp : IComparer<Tasklib>
    {
        public int Compare(Tasklib x, Tasklib y)
        {
            
            return x.DeviceProperty.StartAddress.CompareTo(y.DeviceProperty.StartAddress);
        }
    }

    public class Modbushelp
    {
        baselib _baselib = new baselib();
        //当长度大于125时
        public List<byte> byteSource = new List<byte>();
        public bool GetData(ModbusTCPLib _ModbusLib, HiemsDevicePropertyDto DeviceProperty, int count = 125)
        {
            byteSource.Clear();
            if (DeviceProperty.Length > count &&
               (ToEnum<FunctionEnum>(DeviceProperty.Function) == FunctionEnum.ReadHoldingRegisters ||
               ToEnum<FunctionEnum>(DeviceProperty.Function) == FunctionEnum.ReadInputRegisters))
            {
                int d = (int)DeviceProperty.Length / count;
                int b = (int)DeviceProperty.Length % count;
                bool res;
                ushort startAddress = 0;
                for (int i = 0; i < d; i++)
                {
                    startAddress = (ushort)(DeviceProperty.StartAddress + i * count);
                    res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, startAddress, (ushort)count, ToEnum<FunctionEnum>(DeviceProperty.Function));
                    if (!res) return false;
                    if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                    Thread.Sleep(10);
                }
                if (b != 0)
                {
                    res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, (ushort)(startAddress + count), (ushort)b, ToEnum<FunctionEnum>(DeviceProperty.Function));
                    if (!res) return false;
                    if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                }
                return true;
            }
            else
            {
                bool res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, (ushort)DeviceProperty.StartAddress, (ushort)DeviceProperty.Length, ToEnum<FunctionEnum>(DeviceProperty.Function),
                    DeviceProperty.Value);
                if (!res) return false;
                if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                return res;
            }
        }
        public bool GetData(ModbusRTULib _ModbusLib, HiemsDevicePropertyDto DeviceProperty, int count = 125)
        {
            byteSource.Clear();
            if (DeviceProperty.Length > count &&
               (ToEnum<FunctionEnum>(DeviceProperty.Function) == FunctionEnum.ReadHoldingRegisters ||
               ToEnum<FunctionEnum>(DeviceProperty.Function) == FunctionEnum.ReadInputRegisters))
            {
                int d = (int)DeviceProperty.Length / count;
                int b = (int)DeviceProperty.Length % count;
                bool res;
                ushort startAddress = 0;
                for (int i = 0; i < d; i++)
                {
                    startAddress = (ushort)(DeviceProperty.StartAddress + i * count);
                    res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, startAddress, (ushort)count, ToEnum<FunctionEnum>(DeviceProperty.Function));
                    if (!res) return false;
                    if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                    Thread.Sleep(10);
                }
                if (b != 0)
                {
                    res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, (ushort)(startAddress + count), (ushort)b, ToEnum<FunctionEnum>(DeviceProperty.Function));
                    if (!res) return false;
                    if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                }
                return true;
            }
            else
            {
                bool res = _ModbusLib.ReadAndWrite((ushort)DeviceProperty.SlaveId, (ushort)DeviceProperty.StartAddress, (ushort)DeviceProperty.Length, ToEnum<FunctionEnum>(DeviceProperty.Function),
                    DeviceProperty.Value);
                if (!res) return false;
                if (_ModbusLib._DataByteArray != null) byteSource.AddRange(_ModbusLib._DataByteArray);
                return res;
            }
        }

        /// <summary>
        /// 多个地址合并读取后的数据提取
        /// </summary>
        /// <param name="_ListValues"></param>
        /// <param name="limit"></param>
        /// <param name="_Tasklib"></param>
        /// <returns></returns>
        public ResultLib GetResultData(Dictionary<string, byte[]> _ListValues, string limit, Tasklib _Tasklib)
        {
            string result = string.Empty;
            ResultLib _ResultLib = new();
            try
            {
                string[] limit_key = limit.Split("-");
                byte[] value = _ListValues[limit];
                int SkipCount = _Tasklib.DeviceProperty.StartAddress - Convert.ToInt16(limit_key[0]);
                switch (ToEnum<FunctionEnum>(_Tasklib.DeviceProperty.Function))
                {
                    case FunctionEnum.ReadDiscreteInputs:
                    case FunctionEnum.ReadCoils:
                    case FunctionEnum.ReadHoldingRegisters:
                    case FunctionEnum.ReadInputRegisters:
                        result = _baselib.DataConversion(value, ToEnum<DataFormatEnum>(_Tasklib.DeviceProperty.DataFormat), (int)_Tasklib.DeviceProperty.Length, SkipCount, _Tasklib.DeviceProperty.Formula);
                        break;
                }
            }
            catch (Exception EX)
            {
            }
            _ResultLib.Result = (result == "" ? "NG" : "OK");
            _ResultLib.Value = result;
            return _ResultLib;
        }


            



        //判断刷新间隔是否符合要求
        public bool CheckRefreshInterval(ConcurrentDictionary<string, DateTime> _list, string Router, int RefreshInterval, DateTime borntime)
        {
            try
            {
                if (_list.ContainsKey(Router))
                {
                    // 计算时间间隔
                    TimeSpan ts = borntime.Subtract(_list[Router]);
                    if (ts.TotalMilliseconds >= (RefreshInterval == 0 ? 0 : RefreshInterval))
                    {
                        _list.TryRemove(Router, out DateTime value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return true;
        }

        //将连续地址拼接起来一次性读取（根据功能码区分）       
        public Dictionary<string, Dictionary<string, byte[]>> CreatTask(int AddressInterval, List<Tasklib> _SourceTask)
        {
            if (_SourceTask == null || _SourceTask.Count < 0) return null;
            Dictionary<string, Dictionary<string, byte[]>> list = new Dictionary<string, Dictionary<string, byte[]>>();
            //找出有几种数据类型通过功能码+SlaveID  (四个空调功能码一样但是slaveid不一样不能合并)         
            var modelsGroup = _SourceTask.GroupBy(x => x.DeviceProperty.Function + x.DeviceProperty.SlaveId).ToList();
            foreach (var item in modelsGroup)
            {
                IGrouping<string, Tasklib> models = item;
                List<Tasklib> _Taskliblist = models.ToList();
                //地址排序
                _Taskliblist.Sort(new AddressComp());
                Dictionary<string, byte[]> _result = ReturnContiFields( AddressInterval,_Taskliblist);
                if (_result != null) list.Add(item.Key, _result);
            }
            return list;
        }
        //返回连续字段
        private Dictionary<string, byte[]> ReturnContiFields(int AddressInterval, List<Tasklib> _Taskliblist)
        {
            Dictionary<string, byte[]> list = new Dictionary<string, byte[]>();
            List<int> listAddress = new List<int>();
            for (int i = 0; i < _Taskliblist.Count; i++)
            {
                listAddress.Add(_Taskliblist[i].DeviceProperty.StartAddress);
            }
            var query = listAddress.ToArray().OrderBy(p => p).Aggregate<int, List<List<int>>>(null, (m, n) =>
            {
                if (m == null) return new List<List<int>>() { new List<int>() { n } };
                int cc = m.Last().Last();
                if (n - cc > AddressInterval)
                {
                    m.Add(new List<int>() { n });
                }
                else
                {
                    m.Last().Add(n);
                }
                return m;
            });    
            foreach (var item in query)
            {
                if (item.First()!= item.Last()) list.Add(item.First() + "-" + item.Last(), null);
            }
            return list;         
        }

        //判断是否在范围内
        public bool IsInRange(Tasklib _Tasklib, Dictionary<string, byte[]> _ListValues, out string limit)
        {
            try
            {
                limit = "";
                if (_ListValues == null || _ListValues.Count == 0) return false;
                var value_keys = _ListValues.Keys;
                foreach (var item in value_keys)
                {
                    string result = string.Empty;
                    string[] limit_key = item.Split('-');
                    int address = _Tasklib.DeviceProperty.StartAddress;
                    if (Convert.ToInt16(limit_key[0]) <= address && address <= Convert.ToInt16(limit_key[1]))
                    {
                        limit = item;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                limit = "";
            }
            return false;
        }
        public T ToEnum<T>(string str)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), str);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
