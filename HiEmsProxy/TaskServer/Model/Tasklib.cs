using System;
using HiEMS.Model.Dto;
using HiEMS.Model.Models;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer.Model
{
    public class Tasklib
    {
        //上传发送时间
        public DateTime AddTime { get; set; }     
        
        public string Router { get; set; }
       
        public int InfoId { get; set; }

        public int DeviceId { get; set; }

        public int LocalId { get; set; }

        public string Label { get; set; }

        public string Result { get; set; }

        public string  Value { get; set; }

        public int TaskType { get; set; } //0:采集    1:执行   2:策略

        public HiemsDeviceProtocol DeviceProtocol { get; set; }
    
        public HiemsDevicePropertyDto DeviceProperty { get; set; }
    }
}
