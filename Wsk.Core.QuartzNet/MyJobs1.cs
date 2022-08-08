using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsk.Core.QuartzNet
{
    [DisallowConcurrentExecution]
    internal class MyJobs1:IJob
    {
        public Task Execute(IJobExecutionContext content)
        {
            Console.WriteLine($"{DateTime.Now}>>>>my 333  quartz jobs");
             return Task.CompletedTask;
        }
    }
}
