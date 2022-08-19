using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HiEmsProxy.TaskServer.Actuator
{
    public class ConStateModel
    {
        public int ProtocolId { get; set; }  //协议id  
        public string Description { get; set; }  //描述
    }
    public  class Common
    {
        //各个设备的连接状态
        public void DeviceConState(int protocolId, string description)
        {
            if (DelegateLib.SignalDevConDelegate != null)
            {
                ConStateModel _ConStateModel = new ConStateModel()
                {
                    ProtocolId = protocolId,
                    Description = description
                };
                string result = JsonConvert.SerializeObject(_ConStateModel);
                DelegateLib.SignalDevConDelegate(result);
            }
        }
        //任务回调
        public void UploadData(Tasklib _Tasklib, ResultLib _ResultLib)
        {
            _Tasklib.AddTime = DateTime.Now;
            if (_ResultLib.Value != null) _Tasklib.Value = _ResultLib.Value;
            _Tasklib.Result = _ResultLib.Result;
            switch (_Tasklib.TaskType)
            {
                case 0://采集
                     UploadEvent(_Tasklib);
                    break;
                case 1://远程执行
                    ExecuteEvent(_Tasklib);
                    UploadEvent(_Tasklib);
                    break;
                case 2://策略执行
                    StrategyEvent(_Tasklib);
                    break;
            }
        }
        private void ExecuteEvent(Tasklib _Tasklib)
        {  
            //执行结果回调
            if (DelegateLib.ExecuteDelegate != null)
            {
                DelegateLib.ExecuteDelegate(_Tasklib);
            }
        }
        private void UploadEvent(Tasklib _Tasklib)
        { 
            //采集数据上传回调
            if (DelegateLib.UploadDelegate != null)
            {
                DelegateLib.UploadDelegate(_Tasklib);
            }
        }
        private void StrategyEvent(Tasklib _Tasklib)
        {  //策略执行结果
            if (DelegateLib.StrategyDelegate != null)
            {
                DelegateLib.StrategyDelegate(_Tasklib);
            }
        }
    }
}
