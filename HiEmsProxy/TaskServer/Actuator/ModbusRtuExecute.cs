using ModbusLibNew;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer.Actuator
{
    /// <summary>
    /// modbus rtu 执行器
    /// </summary>
    public class ModbusRtuExecute: ActInterface
    {       
        baselib _baselib = new baselib();
        public ConcurrentDictionary<string, DateTime> _list = new();
        readonly Modbushelp _modbushelp = new();
        public ModbusRTULib _ModbusLib = new ModbusRTULib();
        //执行集合
        public BlockingCollection<Tasklib> _ExcuteBlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);
        //采集集合
        public List<Tasklib> _TaskList { get; set; } = new List<Tasklib>();
        public BlockingCollection<Tasklib> _BlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);       
        public ModbusRtuExecute(SerialPort _SerialPort)
        {
             bool res = _ModbusLib.Init(_SerialPort);
             if(res) Main();
        }
        //开始任务
        public void Main()
        {
            Task.Factory.StartNew(() =>
            {
                DelegateLib.manual.WaitOne();
                while (true)
                {                   
                    Dictionary<string, Dictionary<string, byte[]>> _Addresslist = CreatNewTask(_TaskList);                    
                    Thread.Sleep(10);
                    for (int i = 0; i < _TaskList.Count; i++)
                    {                      
                        Tasklib _Tasklib = null;
                        ResultLib _ResultLib = null;
                        if (_ExcuteBlockingCollection.Count > 0)
                        {
                            //优先执行
                            _Tasklib = _ExcuteBlockingCollection.Take();
                            _ResultLib = Read(_Tasklib.DeviceProperty);
                            _modbushelp.ExecuteEvent(_ResultLib);
                            _ResultLib.Router = _Tasklib.Router;
                            continue;
                        }
                        else
                        {
                            if (_TaskList.Count > i) _Tasklib = _TaskList[i];
                            // 判断是否在刷新间隔内
                            bool res = _modbushelp.CheckRefreshInterval(_list, _Tasklib.Router, _Tasklib.DeviceProperty.RefreshRate, DateTime.Now); if (!res) continue;
                            //判断任务地址是否在已读取的地址列表中
                            Dictionary<string, byte[]> _ListValues = null;
                            if (_Addresslist != null && _Addresslist.ContainsKey(_Tasklib. DeviceProperty.Function + _Tasklib.DeviceProperty.SlaveId)) _ListValues = _Addresslist[_Tasklib.DeviceProperty.Function + _Tasklib.DeviceProperty.SlaveId];
                            string limit;
                            res = _modbushelp.IsInRange(_Tasklib, _ListValues, out limit);
                            if (res)
                            {
                                _ResultLib = _modbushelp.GetResultData(_ListValues, limit, _Tasklib);                                                           
                            }
                            else
                            {
                                _ResultLib = Read(_Tasklib.DeviceProperty);
                            }
                            _list.TryAdd(_Tasklib.Router, DateTime.Now);
                        }                       
                        //数据上传
                        if (_ResultLib != null)
                        {
                            _ResultLib.Router = _Tasklib.Router;
                            _Tasklib.BronTime = DateTime.Now;
                            //数据上传回调
                            _modbushelp.UploadEvent(_ResultLib);
                        }
                    }
                }
            });
        }

        //读取和写入方法
        private ResultLib Read(HiemsDeviceProperty DeviceProperty, bool IsDataConversion = true)
        {
            ResultLib _ResultLib = new();
            try
            {              
                _ResultLib.Result = "NG";
                _ResultLib.Value = "";
                if (_ModbusLib._RTUClient == null || !_ModbusLib._RTUClient.IsConnected)
                {
                    bool res = _ModbusLib.ReConnect();
                    if (!res)
                    {
                        _ResultLib.Result = "NG";
                        _ResultLib.Value = "";
                        _ResultLib.RW = DeviceProperty.RwType;
                        return _ResultLib;
                    }
                }
                //执行方法 
                bool result = _modbushelp.GetData(_ModbusLib, DeviceProperty);
                _ResultLib.Result = result ? "OK" : "NG";
                //数据转换
                _ResultLib.ValueByteArray = _modbushelp.byteSource.ToArray();
                if(IsDataConversion)  _ResultLib.Value = _baselib.DataConversion(_modbushelp.byteSource.ToArray(),  _modbushelp.ToEnum<DataFormatEnum>(DeviceProperty.DataFormat), DeviceProperty.Length, 0,DeviceProperty.Formula);
                _ResultLib.RW = DeviceProperty.RwType;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return _ResultLib;
        }

        //将连续地址拼接起来一次性读取（根据功能码区分）    
        private readonly int interval = 10;
        public Dictionary<string, Dictionary<string, byte[]>> CreatNewTask(List<Tasklib> _SourceTask)
        {
            Dictionary<string, Dictionary<string, byte[]>> list = new Dictionary<string, Dictionary<string, byte[]>>();
            //找出有几种数据类型通过功能码 +slaveID         
            var modelsGroup = _SourceTask.GroupBy(x => x.DeviceProperty.Function+ x.DeviceProperty.SlaveId).ToList();
            foreach (var item in modelsGroup)
            {
                IGrouping<string, Tasklib> models = item;
                List<Tasklib> _Taskliblist = models.ToList();
                //地址排序
                _Taskliblist.Sort(new AddressComp());
                Dictionary<string, byte[]> _result = ReturnContiFields(_Taskliblist);
                if (_result != null) list.Add(item.Key, _result);                        
            }
            return list;
        }
        //返回连续字段
        private Dictionary<string, byte[]> ReturnContiFields(List<Tasklib> _Taskliblist)
        {
            if (_Taskliblist.Count == 0) return null;
            Dictionary<string, byte[]> _listDeviceData = new Dictionary<string, byte[]>();
            int start = _Taskliblist[0].DeviceProperty.StartAddress;
            int value = _Taskliblist[0].DeviceProperty.StartAddress;
            int address = 0;
            int length = 1;
            int SlaveID = _Taskliblist[0].DeviceProperty.SlaveId;
            byte[] result = null;
            FunctionEnum Function = _modbushelp.ToEnum<FunctionEnum>(_Taskliblist[0].DeviceProperty.Function);
            for (int i = 0; i < _Taskliblist.Count; i++)
            {
                address = _Taskliblist[i].DeviceProperty.StartAddress;
                length = _Taskliblist[i].DeviceProperty.Length;
                if (address - value > interval)
                {
                    result = GetValue(SlaveID, Function, start, value - start + 1 * _Taskliblist[i - 1].DeviceProperty.Length);
                    if (result != null) _listDeviceData.Add($"{start}-{value + 1 * _Taskliblist[i - 1].DeviceProperty.Length - 1}", result);
                    value = address;
                    start = address;
                }
                else
                {
                    value = address;
                }
            }
            if ((address - start + 1) != 1)
            {
                result = GetValue(SlaveID, Function, start, address - start + 1 * length);
               if(result!=null) _listDeviceData.Add($"{start}-{address + 1 * length - 1}", result);
            }
            return _listDeviceData;
        }
        private byte[] GetValue(int SlaveID, FunctionEnum Function, int StartAddress, int length)
        {
            string limits = $"{StartAddress}-{StartAddress + length - 1}";
            ResultLib _ResultLib = Read(new HiemsDeviceProperty()
            {
                SlaveId = SlaveID,
                Function = Enum.GetName(typeof(FunctionEnum), Function),
                StartAddress = StartAddress,
                Length = length,
            }, false);
            if (_ResultLib.Result == "OK")
            {
                return _ResultLib.ValueByteArray;
            }
            return null;
        }
    } 
}

