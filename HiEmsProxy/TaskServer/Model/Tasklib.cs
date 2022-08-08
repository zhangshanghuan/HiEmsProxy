using System;
using HiEMS.Model.Dto;
using HiEMS.Model.Models;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer.Model
{
    public class Tasklib
    {
        //上传发送时间
        public DateTime BronTime { get; set; }     
        
        public string Router { get; set; }
       
        public int infoId { get; set; }

        public int deviceId { get; set; }

        public int localId { get; set; }

        public string label { get; set; }

        public string Result { get; set; }

        public string ResultValue { get; set; }

        public int TaskType { get; set; } //0:采集    1:执行

        public HiemsDeviceProtocol DeviceProtocol { get; set; }
    
        public HiemsDevicePropertyDto DeviceProperty { get; set; }
    }
}
