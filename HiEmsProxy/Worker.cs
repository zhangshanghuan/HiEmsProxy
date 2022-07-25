using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer;


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
            //  string[] ddd = new string[] { "12", "13" };
            //var dd=   Convert.ToUInt16(ddd[0]) << 16 + Convert.ToUInt16(ddd[1]);
            //     string address = "12";
            //   ushort dd=  Convert.ToUInt16(address);
            //EvalHelp _EvalHelp = new EvalHelp();
            //string str=   _EvalHelp.EvalMian("10", "(8>2&&7>8)? true:false");
            //  SignalClien _SignalClien = new SignalClien();
            await Task.Run(() =>
            {
                HiemsMain _Common = new HiemsMain();
                _Common.Start();
            });
        }
    }
}