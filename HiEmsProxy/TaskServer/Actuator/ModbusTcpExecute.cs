using ModbusLibNew;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using Microsoft.Extensions.Configuration;
using HiEMS.Model.Models;
using HiEMS.Model.Dto;

namespace HiEmsProxy.TaskServer.Actuator
{
    public class ModbusTcpExecute:ActInterface
    {
        Common _common = new Common();
        baselib _baselib = new baselib();
        public ConcurrentDictionary<string, DateTime> _list = new();   
        public ModbusTCPLib _ModbusLib=new ModbusTCPLib();
        readonly Modbushelp _modbushelp = new();
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        BaseConfig _BaseConfig = null;
        bool ConState = false;

        //执行集合
        public BlockingCollection<Tasklib> _ExcuteBlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);
        //采集集合
        public List<Tasklib> _TaskList { get; set; } = new List<Tasklib>();
        public BlockingCollection<Tasklib> _BlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);
        public int ID;  
        public ModbusTcpExecute(string ip, int port, int iD)
        {
            ID = iD;
            ConState = _ModbusLib.Init(ip, port);
            if (ConState) Main();         
            _common.DeviceConState(ID, ConState);          
           _BaseConfig = config.GetRequiredSection("BaseConfig").Get<BaseConfig>();        
        }
        //开始任务
        public void Main()
        {
            _ = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    DelegateLib.manual.WaitOne();
                    //地址拼接集合                     
                    Dictionary<string, Dictionary<string, byte[]>> _Addresslist = _modbushelp.CreatTask(_BaseConfig.AddressInterval, _TaskList);                 
                    Thread.Sleep(100);
                    //先批量读取
                    for (int i = 0; i < _TaskList.Count; i++)
                    {
                        Tasklib _Tasklib = null;
                        ResultLib _ResultLib = null;
                        if (_ExcuteBlockingCollection.Count > 0)
                        {
                            //优先执行
                            _Tasklib = _ExcuteBlockingCollection.Take();
                            _ResultLib = Read(_Tasklib.DeviceProperty);
                            _ResultLib.Value = _Tasklib.DeviceProperty.Value;
                        }
                        else
                        {
                            if (_TaskList.Count > i) _Tasklib = _TaskList[i];
                            string key = _Tasklib.DeviceProperty.Function + _Tasklib.DeviceProperty.SlaveId;
                            // 判断是否在刷新间隔内
                            bool res = _modbushelp.CheckRefreshInterval(_list, _Tasklib.Router, (int)_Tasklib.DeviceProperty.Refresh, DateTime.Now); if (!res) continue;
                            //判断任务地址是否在已读取的地址列表中                          
                            if (_Addresslist != null && _Addresslist.ContainsKey(key))
                            {
                                Dictionary<string, byte[]> limits = _Addresslist[key];
                                //判断存在范围
                                string limit;
                                if (_modbushelp.IsInRange(_Tasklib, limits, out limit))
                                {
                                    //读取值                               
                                    int startadd = Convert.ToInt32(limit.Split('-')[0]);
                                    int end = Convert.ToInt32(limit.Split('-')[1]);
                                    int length = end - startadd + 1 * (int)_Tasklib.DeviceProperty.Length;                                
                                    if (limits[limit] == null) limits[limit] = GetValue((int)_Tasklib.DeviceProperty.SlaveId, _modbushelp.ToEnum<FunctionEnum>(_Tasklib.DeviceProperty.Function), startadd, length);
                                    if (limits[limit] != null)
                                    {
                                        _ResultLib = _modbushelp.GetResultData(limits, limit, _Tasklib);
                                    }
                                    else
                                    {
                                        _ResultLib = Read(_Tasklib.DeviceProperty);
                                    }
                                }
                                else
                                {
                                    _ResultLib = Read(_Tasklib.DeviceProperty);
                                }
                            }
                            _list.TryAdd(_Tasklib.Router, DateTime.Now);
                        }
                        //数据上传
                        if (_ResultLib != null && _Tasklib != null)
                        {
                            _common.UploadData(_Tasklib, _ResultLib);
                        }
                    }
                }
            });
        }
   
        //读取和写入方法
        private ResultLib Read(HiemsDevicePropertyDto DeviceProperty, bool IsDataConversion = true)
        {
            ResultLib _ResultLib = new();
            try
            {
                //判断是否连接成功
                if (_ModbusLib._TcpClient == null || !_ModbusLib._TcpClient.IsConnected)
                {
                     ConState = _ModbusLib.ReConnect();
                    if (!ConState)
                    {
                        _common.DeviceConState(ID, ConState);
                        _ResultLib.Result = "NG";
                        _ResultLib.Value = "";
                        return _ResultLib;
                    }
                }
                _common.DeviceConState(ID, ConState);
                //执行方法 
                bool res = _modbushelp.GetData(_ModbusLib, DeviceProperty, _BaseConfig != null ? _BaseConfig.ModbusMaxCount : 125);
                _ResultLib.Result = res ? "OK" : "NG";
                if (res)
                {
                    _ResultLib.ValueByteArray = _modbushelp.byteSource.ToArray();
                    if (IsDataConversion) _ResultLib.Value = _baselib.DataConversion(_modbushelp.byteSource.ToArray(), _modbushelp.ToEnum<DataFormatEnum>(DeviceProperty.DataFormat), (int)DeviceProperty.Length, 0, DeviceProperty.Formula);
                }
            }
            catch (Exception)
            {
            }
            return _ResultLib;
        }
  
        private byte[] GetValue(int SlaveID, FunctionEnum Function, int StartAddress, int length)
        {              
            ResultLib _ResultLib = Read(new HiemsDevicePropertyDto()
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
