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
        
        
        public static Action<string> SignalDevicePropDelegate;
        public static Action<string> SignalRemoteDelegate;

        //设备连接委托
        public static Action<string> SignalDevConDelegate;


    }
}
