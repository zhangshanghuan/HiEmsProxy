using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;

namespace HithiumEmsPlatForm.BLL.TaskServer
{
    /// <summary>
    /// 数据上传服务
    /// </summary>
    public  class UpLoadLib
    {
        public  BlockingCollection<ResultLib> L_listTask = new BlockingCollection<ResultLib>(10000);
        //上传优先级队列
        public PriorityQueue<ResultLib, int> _UploadPriorityQueue = new PriorityQueue<ResultLib, int>();
        public UpLoadLib()
        {
            DelegateLib.UploadDelegate += Receive;
            Task.Run(() => {
                RUN();
            });         
        }
        //收到路由和值      
        //数据上传缓存
        //数据传输失败
        long count = 0;
        public void RUN()
        {
            try
            {
                while (true)
                {
                    DelegateLib.manual.WaitOne();
                    for (int i = 0; i < L_listTask.Count; i++)
                    {                    
                        ResultLib RESULT = L_listTask.Take();
                        Console.WriteLine($"{RESULT.Router}  {RESULT.RW}  ##{RESULT.Value} ## {RESULT.Result}");
                       //JsonCommon.SetJsonStrKey1(RESULT.Router, "Value", RESULT.Value, ref JsonCommon.NewSnapshot);
                      //string ddd = JsonCommon.GetJsonStrKey1(RESULT.Router, "Value", JsonCommon.NewSnapshot);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据上传服务报错：" + ex.ToString());
            }
        }

        //委托回调
        public   void Receive(ResultLib _Tasklib)
        {
            try
            {              
                L_listTask.Add(_Tasklib);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据上传服务报错：" + ex.ToString());
            }
        }
    }
}
