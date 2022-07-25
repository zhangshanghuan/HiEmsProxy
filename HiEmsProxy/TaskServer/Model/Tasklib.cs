using System;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer.Model
{
    public class Tasklib
    {
        //上传发送时间
        public DateTime BronTime { get; set; }
        //任务寿命
        public DateTime SMTime { get; set; }
        //路由(传输路由)
        public string Router { get; set; }
        public HiemsDeviceProtocol DeviceProtocol { get; set; }
        public HiemsDeviceProperty DeviceProperty { get; set; }
    }
}
