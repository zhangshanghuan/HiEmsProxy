using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsk.Core.QuartzNet
{
    public static class QuartzJobService
    {
        public static void AddQuartzJobService(this IServiceCollection services,string name)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
           services.AddSingleton<IJobFactory, WeskyJobFactory>();
           services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<MyJobs>();
            services.AddSingleton(
                new WeskyJobSchedule(typeof(MyJobs), "0/1 * * * * ? ","1")
          );
            services.AddSingleton<MyJobs1>();    
            services.AddSingleton(
            new WeskyJobSchedule(typeof(MyJobs1), "0/3 * * * * ? ","2")
          );          
          IServiceCollection dd=  services.AddHostedService<WeskyJobHostService>();
        }
    }
}
