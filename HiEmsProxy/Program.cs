using HiEMS.WebApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Wsk.Core.QuartzNet;

namespace HiEmsProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();       
        }
     
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);
            //�жϵ�ǰϵͳ�Ƿ�Ϊwindows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                host.UseWindowsService();
            }
            return host.ConfigureServices((hostContext, services) =>
            {          
                 services.AddHostedService<Worker>();
                //���һ����ʱ����
                services.AddTaskSchedulers();
            });
        }
    }
}
