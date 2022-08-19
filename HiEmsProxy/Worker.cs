using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer;
using ModbusLibNew;
using Microsoft.Extensions.Configuration;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using Newtonsoft.Json;
using System;
using HiEmsProxy.Quartz;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace HiEmsProxy
{
   
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            //BaseConfig _BaseConfig = config.GetRequiredSection("BaseConfig").Get<BaseConfig>();
            //  string[] ddd = new string[] { "12", "13" };
            //var dd=   Convert.ToUInt16(ddd[0]) << 16 + Convert.ToUInt16(ddd[1]);
            //     string address = "12";
            //   ushort dd=  Convert.ToUInt16(address);
            // EvalHelp _EvalHelp = new EvalHelp();
            //string str = _EvalHelp.EvalMian("10", "(8>2&&7>8)? true:false");
            //string str = _EvalHelp.EvalMian("10", "raw|0x3");  //1010  0001
            //
            await Task.Run(() =>
            {            
                //实现任务添加
                //var dd = Provider._ServiceProvider.GetService<ITaskSchedulerServer>();
                //SysTasksQz _SysTasksQz = new SysTasksQz()
                //{
                //    JobGroup = "1",
                //    Name = "zsh",
                //    AssemblyName = "HiEmsProxy",
                //    ClassName = "TestJob",
                //    ID = "myjob",
                //    BeginTime = DateTime.Now,
                //    EndTime = DateTime.Now.AddSeconds(10),
                //    IntervalSecond = 1,
                //};
                //// 3.创建 job
                //JobBuilder job = JobBuilder.Create<TestJob>();
                //dd.AddTaskScheduleAsync(job, _SysTasksQz);
             HiemsMain _Common = new HiemsMain();
            _Common.Start();
        });
        }
    }
}