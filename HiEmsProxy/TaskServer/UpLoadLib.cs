using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
using HiEMS.Model.Models;
using HiEmsProxy;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HiEmsProxy.TaskServer.Model.DifferenceComparisonlib;

namespace HithiumEmsPlatForm.BLL.TaskServer
{
    /// <summary>
    /// 数据上传服务
    /// </summary>
    public  class UpLoadLib
    {
        private object _lock = new object();
        //上传集合
        public List<DataCollectHubVo> _UploadList;
        private List<DataCollectHubVo> _OldUploadList = new List<DataCollectHubVo>();
        //定义缓存集合list
        private List<DataCollectHubVo> _CacheList = new List<DataCollectHubVo>();
        //定义一个状态集合
        private List<DataCollectHubVo> _StateList = new List<DataCollectHubVo>();
        private string UpLoadJson = string.Empty;
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        BaseConfig _BaseConfig = null;
        private int uploadTime = 5000;
        SignalClien signalClien=null;
        public  System.Timers.Timer  _Timer = new System.Timers.Timer();
        public UpLoadLib()
        {
            signalClien = SignalClien.getInstance();
            DelegateLib.UploadDelegate += UploadDelegate;
            _BaseConfig = config.GetRequiredSection("BaseConfig").Get<BaseConfig>();
            uploadTime = _BaseConfig != null ? _BaseConfig.UploadInterval : uploadTime;
            //加载本地缓存值
            string json = JsonCommon.ReadCacheFile();
            _StateList = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(json);        
            //设置定时上传服务
            SetTimer();
            DelegateLib.DevConDelegate += DevConnectDelegate;
        }
        //定时上传数据给网关
        private  void SetTimer()
        {
            _Timer.Interval = uploadTime;
            _Timer.Elapsed += ATimer_Elapsed; ;
            _Timer.AutoReset = true;
            _Timer.Enabled = false;
        }
        //定时上传事件
        private   void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {       
                //比较对象是否有差异
               Result res = DifferenceComparisonlib.ComPare<List<DataCollectHubVo>>(_OldUploadList, _UploadList);
               if (res.IsEqual) return;
                AddUploadList(null, true);              
                for (int i = 0; i < 1; i++)
                {    
                    bool result = signalClien != null? signalClien.SetDeviceData(UpLoadJson):false;
                    if (result) break;          
                    Console.Write($"上传失败重新上传{i + 1}\r\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据上传服务报错：" + ex.ToString());
            }
        }
        //委托回调
        public void UploadDelegate(Tasklib _Tasklib)
        {
            lock (this)
            {
                try
                {
                    Console.WriteLine($"{_Tasklib.Router} {_Tasklib.Result}");
                    if (_Tasklib.TaskType == 0 && _Tasklib.Result == "NG") return;//采集任务 测试结果ng不上传
                    AddCacheList(_Tasklib);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("上传回调报错：" + ex.ToString());
                }
            }
        }

        //将数据添加进缓存集合
        public void AddCacheList(Tasklib _Tasklib)
        {
            if (_Tasklib == null) return;
            int infoId = _Tasklib.InfoId;
            int PropertyId = _Tasklib.DeviceProperty.Id;
            string value = _Tasklib.Value;
            string result = _Tasklib.Result;
            string rwtype = _Tasklib.DeviceProperty.RwType;
            string Type = _Tasklib.DeviceProperty.Type;
            //遍历缓存集合                  
            for (int i = 0; i < _CacheList.Count; i++)
            {
                //检查是否包含此设备集合
                if (_CacheList[i].InfoId == infoId)
                {
                    for (int j = 0; j < _CacheList[i].Data.Count; j++)
                    {
                        //检查是否包含次测试项目
                        if (PropertyId == _CacheList[i].Data[j].PropertyId)
                        {
                            //当值发生改变时将数据添加进上传集合
                            if (_CacheList[i].Data[j].Value != value)
                            {
                                AddUploadList(_Tasklib);
                            }
                            _CacheList[i].Data.RemoveAt(j);
                            _CacheList[i].Data.Add(TaskToHiemsDeviceDataDto(_Tasklib));
                            return;
                        }
                    }
                    _CacheList[i].Data.Add(TaskToHiemsDeviceDataDto(_Tasklib));
                    AddUploadList(_Tasklib);
                    return;
                }
            }
            _CacheList.Add(TaskToDataCollectHubVo(_Tasklib));
            AddUploadList(_Tasklib);
        }
        //上传网关装载对象
        public void AddUploadList(Tasklib _Tasklib, bool IsRemove=false)
        {          
            lock (_lock)
            {
                try
                {
                    if (_UploadList==null||_UploadList.Count == 0) return;
                    if (IsRemove)
                    {
                        UpLoadJson = JsonConvert.SerializeObject(_UploadList, Formatting.Indented);
                        for (int j = 0; j < _UploadList.Count; j++)
                        {
                            if (_UploadList[j].Data != null) _UploadList[j].Data.Clear();
                        }
                        _OldUploadList = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(JsonConvert.SerializeObject(_UploadList, Formatting.Indented));
                    }
                    else
                    {
                        #region  将变动的数据添加到上传对象                    
                        //默认为fasle的报警不上传网关，除非缓存中有之前触发了报警需要解除
                        if (!IsExit(_Tasklib) && _Tasklib.DeviceProperty.Type == "ALARM" && _Tasklib.Value == "False") return;
                        if (IsExit(_Tasklib) && _Tasklib.DeviceProperty.Type == "ALARM" && _Tasklib.Value == "True") return;
                        //将报警加入缓存，将报警解除移除
                        AddStateData(_Tasklib);
                        int infoId = _Tasklib.InfoId;
                        int PropertyId = _Tasklib.DeviceProperty.Id;
                        string value = _Tasklib.Value;
                        string result = _Tasklib.Result;
                        string Type = _Tasklib.DeviceProperty.Type;
                        //遍历缓存集合
                        for (int i = 0; i < _UploadList.Count; i++)
                        {
                            //检查是否包含此设备
                            if (_UploadList[i].InfoId == infoId)
                            {
                                if (_UploadList[i].Data == null) _UploadList[i].Data = new List<HiemsDeviceDataDtoForHub>();
                                //检查设备是否包含次测试项目
                                for (int j = 0; j < _UploadList[i].Data.Count; j++)
                                {
                                    if (PropertyId == _UploadList[i].Data[j].PropertyId)
                                    {
                                         //更新测试项目
                                        _UploadList[i].Data.RemoveAt(j);
                                        _UploadList[i].Data.Add(TaskToHiemsDeviceDataDto(_Tasklib));
                                        return;
                                    }
                                }
                                //添加测试测试项目
                                _UploadList[i].Data.Add(TaskToHiemsDeviceDataDto(_Tasklib));
                                return;
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        #region 设备连接状态
        private void DevConnectDelegate(HiemsDeviceInfo _HiemsDeviceInfo)
        {
            UpdateDeviceState((int)_HiemsDeviceInfo.ProtocolId, _HiemsDeviceInfo.State);
        }
        //修改设备通讯状态
        public void UpdateDeviceState(int protocolId, string state)
        {
            for (int i = 0; i < _UploadList.Count; i++)
            {
                if (_UploadList[i].ProtocolId == protocolId)
                {
                   if (_UploadList[i].Info.State != state)
                    {
                        _UploadList[i].Info.State = state;
                    }
                }
            }
        }

        #endregion

        //将告警状态数据存入缓存
        public void AddStateData(Tasklib Tasklib)
        {
            if (Tasklib.DeviceProperty.Type == "ALARM" && Tasklib.Value == "True")
            {
                _StateList.Add(TaskToDataCollectHubVo(Tasklib));
                JsonCommon.WriteCacheFile(JsonConvert.SerializeObject(_StateList));
            }
            if (Tasklib.DeviceProperty.Type == "ALARM" && Tasklib.Value == "False")
            {
                for (int i = 0; i < _StateList.Count; i++)
                {
                    if (_StateList[i].InfoId == Tasklib.InfoId)
                    {
                        for (int j = 0; j < _StateList[i].Data.Count; j++)
                        {
                            if (_StateList[i].Data[j].PropertyId == Tasklib.DeviceProperty.Id)
                            {
                                _StateList.RemoveAt(i);
                                JsonCommon.WriteCacheFile(JsonConvert.SerializeObject(_StateList));
                                return;
                            }
                        }
                    }
                }
            }
        }
        //判断缓存中是否有包含该报警
        public bool IsExit(Tasklib tasklib)
        {
            for (int i = 0; i < _StateList.Count; i++)
            {
                if (_StateList[i].InfoId == tasklib.InfoId)
                {
                    for (int j = 0; j < _StateList[i].Data.Count; j++)
                    {
                        if (_StateList[i].Data[j].PropertyId == tasklib.DeviceProperty.Id&& _StateList[i].Data[j].Value=="True")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //判断集合中是否包含该测试项目
        public bool IsContainHiemsDeviceDataDto(List<DataCollectHubVo> _DataCollectHubVo)
        {
            for (int i = 0; i < _DataCollectHubVo.Count; i++)
            {
                if (_DataCollectHubVo[i].Data.Count>0)
                {
                    return true;
                }
            }
            return false;
        }
        //task to DataCollectHubVo 对象
        public DataCollectHubVo TaskToDataCollectHubVo(Tasklib _tasklib)
        {
            DataCollectHubVo dataCollectHubVo = new DataCollectHubVo();
            dataCollectHubVo.InfoId = _tasklib.InfoId;
            dataCollectHubVo.DeviceId = _tasklib.DeviceId;
            dataCollectHubVo.LocalId = _tasklib.LocalId;
            dataCollectHubVo.Label = _tasklib.Label;
            dataCollectHubVo.Info = _tasklib.Info;
            dataCollectHubVo.Data = new List<HiemsDeviceDataDtoForHub>();
            dataCollectHubVo.Data.Add(TaskToHiemsDeviceDataDto(_tasklib));
            return dataCollectHubVo;
        }
        //task  to HiemsDeviceDataDto对象
        public HiemsDeviceDataDtoForHub TaskToHiemsDeviceDataDto(Tasklib _tasklib)
        {
            HiemsDeviceDataDtoForHub _HiemsDeviceDataDtoForHub = new HiemsDeviceDataDtoForHub()
            {                             
                InfoId = _tasklib.InfoId,
                LocalId = _tasklib.LocalId,
                PropertyId = (int)_tasklib.DeviceProperty.Id,
                Value = _tasklib.Value,
                Result = _tasklib.Result,
                RwType = _tasklib.DeviceProperty.RwType,
                AddTime = _tasklib.AddTime,
                Name = _tasklib.DeviceProperty.Name,
                Describe = _tasklib.DeviceProperty.Describe,
                Dict = _tasklib.DeviceProperty.Dict,
                Type = _tasklib.DeviceProperty.Type,
                Rank = _tasklib.DeviceProperty.Rank,
                Gl1 = _tasklib.DeviceProperty.Gl1,
                Gl2 = _tasklib.DeviceProperty.Gl2,
                Gl3 = _tasklib.DeviceProperty.Gl3,
                Gl4 = _tasklib.DeviceProperty.Gl3,
                Attach ="起始地址："+ _tasklib.DeviceProperty.StartAddress,
            };
            return _HiemsDeviceDataDtoForHub;
        }
    }
}
