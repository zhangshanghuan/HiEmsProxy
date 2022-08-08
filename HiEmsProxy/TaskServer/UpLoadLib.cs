using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
using HiEmsProxy;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HithiumEmsPlatForm.BLL.TaskServer
{
    /// <summary>
    /// 数据上传服务
    /// </summary>
    public  class UpLoadLib
    {
        private object _lock = new object();

        //定义缓存集合list
        public List<DataCollectHubVo> _CacheList = new List<DataCollectHubVo>();
        //上传集合
        public List<DataCollectHubVo> _UploadList = new List<DataCollectHubVo>();
        public string UpLoadJson = string.Empty;

        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        BaseConfig _BaseConfig = null;
        public int uploadTime = 5000;
        SignalClien signalClien;
        public UpLoadLib(SignalClien _SignalClien)
        {
            signalClien = _SignalClien;
            DelegateLib.UploadDelegate += UploadDelegate;
            _BaseConfig = config.GetRequiredSection("BaseConfig").Get<BaseConfig>();
            uploadTime = _BaseConfig != null ? _BaseConfig.UploadInterval : uploadTime;
            SetTimer();
        }
        //定时上传数据给网关
        private  void SetTimer()
        {
            // Create a timer with a two second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer(uploadTime);
            // Hook up the Elapsed event for the timer. 
           aTimer.Elapsed += ATimer_Elapsed; ;
           aTimer.AutoReset = true;
           aTimer.Enabled = true;
        }
        //定时上传事件
        private  async void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //将_CacheList加入缓存json
                JsonCommon.CacheSnapshot = JsonCommon.SerializeDataContractJson<List<DataCollectHubVo>>(_CacheList);
               // JsonCommon.WriteCacheFile();                
                if (_UploadList.Count==0||!IsContainHiemsDeviceDataDto(_UploadList)) return;             
                UpLoadJson=String.Empty;
                AddUploadObj(null, true);
                if (UpLoadJson == String.Empty) return;              
                for (int i = 0; i < 3; i++)
                {
                    bool result = await signalClien.SetDeviceData(UpLoadJson);
                    if (result) break;
                    Console.Write($"上传失败重新上传{i + 1}");
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
            try
            {           
              AddCacheList(_Tasklib);
              Console.WriteLine($"{_Tasklib.Router} {_Tasklib.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("上传回调报错：" + ex.ToString());
            }
        }
     
        //将数据添加进缓存集合
        public void AddCacheList(Tasklib _Tasklib)
        {
            if (_Tasklib == null) return;
            int infoId = _Tasklib.infoId;
            int PropertyId = _Tasklib.DeviceProperty.Id;
            int DataIdx = (int)_Tasklib.DeviceProperty.DataIdx;
            string value = _Tasklib.ResultValue;
            string result = _Tasklib.Result;
            string rwtype = _Tasklib.DeviceProperty.RwType;
            string Type = _Tasklib.DeviceProperty.Type;        
            //遍历缓存集合
            for (int i = 0; i < _CacheList.Count; i++)
            {
                //检查是否包含此设备集合
                if (_CacheList[i].InfoId == infoId)
                {
                    List<HiemsDeviceDataDto> _listHiemsDeviceDataDto = _CacheList[i].Data;                
                    for (int j = 0; j < _listHiemsDeviceDataDto.Count; j++)
                    {
                        //检查是否包含次测试项目
                        if (PropertyId == _listHiemsDeviceDataDto[j].PropertyId && DataIdx == _listHiemsDeviceDataDto[j].DataIdx)
                        {
                            //当值发生改变时将数据添加进上传集合
                            if (_listHiemsDeviceDataDto[j].Value != value|| _listHiemsDeviceDataDto[j].RwType!= rwtype)
                            {                              
                                AddUploadObj(_Tasklib);
                            }
                            _listHiemsDeviceDataDto.RemoveAt(j);
                            _listHiemsDeviceDataDto.Add(AddHiemsDeviceDataDto(_Tasklib));
                            return;
                        }
                    }
                    _listHiemsDeviceDataDto.Add(AddHiemsDeviceDataDto(_Tasklib));                
                    AddUploadObj(_Tasklib);
                    return;
                }           
            }          
        }

        //上传网关装载对象
        public void AddUploadObj(Tasklib _Tasklib, bool IsRemove=false)
        {
            lock (_lock)
            {
                try
                {
                    if (IsRemove)
                    {
                        if (_UploadList.Count == 0) return;
                        UpLoadJson = GetJson(_UploadList);
                        for (int j = 0; j < _UploadList.Count; j++)
                        {
                            //_UploadList[j].Data.RemoveRange(0, _UploadList[j].Data.Count);
                            _UploadList[j].Data.Clear();
                        }
                    }
                    else
                    {
                        if (_Tasklib == null) return;
                        int infoId = _Tasklib.infoId;
                        int PropertyId = _Tasklib.DeviceProperty.Id;
                        int DataIdx = (int)_Tasklib.DeviceProperty.DataIdx;
                        string value = _Tasklib.ResultValue;
                        string result = _Tasklib.Result;
                        string Type = _Tasklib.DeviceProperty.Type;
                        //遍历缓存集合
                        for (int i = 0; i < _UploadList.Count; i++)
                        {
                            //检查是否包含此设备
                            if (_UploadList[i].InfoId == infoId)
                            {
                                List<HiemsDeviceDataDto> _listHiemsDeviceDataDto = _UploadList[i].Data;
                                //检查设备是否包含次测试项目
                                for (int j = 0; j < _listHiemsDeviceDataDto.Count; j++)
                                {
                                    if (PropertyId == _listHiemsDeviceDataDto[j].PropertyId && DataIdx == _listHiemsDeviceDataDto[j].DataIdx)
                                    {
                                        //修改
                                        _UploadList[i].Data.RemoveAt(j);
                                        _UploadList[i].Data.Add(AddHiemsDeviceDataDto(_Tasklib));
                                        return;
                                    }
                                }
                                //添加测试测试项目
                                _UploadList[i].Data.Add(AddHiemsDeviceDataDto(_Tasklib));
                                return;
                            }
                        }
                        //添加设备
                        _UploadList.Add(AddDataCollectHubVo(_Tasklib));
                    }
                }
                catch (Exception ex)
                {
                }
            }
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

        //添加一个DataCollectHubVo 对象
        public DataCollectHubVo AddDataCollectHubVo(Tasklib _tasklib)
        {
            DataCollectHubVo dataCollectHubVo = new DataCollectHubVo();
            dataCollectHubVo.InfoId = _tasklib.infoId;
            dataCollectHubVo.DeviceId = _tasklib.deviceId;
            dataCollectHubVo.LocalId = _tasklib.localId;
            dataCollectHubVo.Label = _tasklib.label;          
            dataCollectHubVo.Data = new List<HiemsDeviceDataDto>();
            dataCollectHubVo.Data.Add(AddHiemsDeviceDataDto(_tasklib));
            return dataCollectHubVo;
        }

        //添加一个HiemsDeviceDataDto对象
        public HiemsDeviceDataDto AddHiemsDeviceDataDto(Tasklib _tasklib)
        {
            HiemsDeviceDataDto hiemsDeviceDataDto = new HiemsDeviceDataDto()
            {
                InfoId = _tasklib.infoId,
                LocalId = _tasklib.localId,
                PropertyId = (int)_tasklib.DeviceProperty.Id,
                Value = _tasklib.ResultValue,
                Result = _tasklib.Result,
                RwType = _tasklib.DeviceProperty.RwType,
                AddTime = _tasklib.BronTime,
                DataIdx = _tasklib.DeviceProperty.DataIdx,
                Name = _tasklib.DeviceProperty.Name,
                Describe = _tasklib.DeviceProperty.Describe,
                Attach ="起始地址："+ _tasklib.DeviceProperty.StartAddr,
            };
            return hiemsDeviceDataDto;
        }

        //list集合遍历成json对象
        private string GetJson<T>(List<T> _list)
        {
            try
            {                
                return JsonConvert.SerializeObject(_list, Formatting.Indented);
            }
            catch (Exception ex)
            {

              
            }
            return "";
        }
        private string GetJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

  

    }
}
