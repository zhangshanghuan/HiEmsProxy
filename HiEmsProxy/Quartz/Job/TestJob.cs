using HiEmsProxy.TaskServer;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {        
        public virtual Task Execute(IJobExecutionContext context)
        {
            StrategyExecute _StrategyExecute= StrategyExecute.GetInstance();
            return Console.Out.WriteLineAsync($"Test job工作了 在{DateTime.Now}");
        }
    }
}
