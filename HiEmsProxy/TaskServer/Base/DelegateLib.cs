using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Model;

namespace HiEmsProxy.TaskServer.Base
{
    /// <summary>
    /// 委托类
    /// </summary>
    public class DelegateLib
    {
        //数据上传委托
        public static Action<Tasklib> UploadDelegate;
       
        //reset 暂停  set 启动
        public static ManualResetEvent manual = new ManualResetEvent(true);

        //执行结果回调
        public static Action<Tasklib> ExecuteDelegate;

        //策略触发事件
        public static Action<string> StrategyTriggerDelegate;
        //策略执行结果
        public static Action<Tasklib> StrategyDelegate;
        //策略执行
        public static Action<Tasklib> StrategyExecuteDelegate;


        //设备属性json
        public static Action<string> SignalDevicePropDelegate;
      
        //远端执行设备属性json
        public static Action<string> SignalRemoteDelegate;

        //设备连接委托
        public static Action<string> SignalDevConDelegate;


    }
}
