using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer
{
    public class RemoteExecute
    {
        private RemoteExecute()
        {
            DelegateLib.ExecuteDelegate += ExecuteMain;
        }
        //单例模式只初始化一次
        private static RemoteExecute instance = null;
        public static RemoteExecute getInstance()
        {
            if (instance == null)
            {
                instance = new RemoteExecute();
            }
            return instance;
        }
        public void ExecuteMain(Tasklib _Tasklib)
        {
           if(_Tasklib != null) Console.WriteLine("远端执行结果:" + _Tasklib.Value);

        }
    }
}
