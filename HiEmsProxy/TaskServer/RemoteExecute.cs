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
        public RemoteExecute()
        {
            DelegateLib.ExecuteDelegate += ExecuteMain;
        }

        public void ExecuteMain(ResultLib _ResultLib)
        {

           if(_ResultLib!=null) Console.WriteLine("远端执行结果:" + _ResultLib.Result);

        }
    }
}
