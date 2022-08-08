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
            return Console.Out.WriteLineAsync($"job工作了 在{DateTime.Now}");
        }
    }
}
