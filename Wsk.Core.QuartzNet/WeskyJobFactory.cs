using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;


namespace Wsk.Core.QuartzNet
{
    public class WeskyJobFactory:IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public WeskyJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            // return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            try { 
                var serviceScope = _serviceProvider.CreateScope();
                var job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);           
                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
