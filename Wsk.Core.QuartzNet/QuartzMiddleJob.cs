using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Wsk.Core.QuartzNet
{
    public class QuartzMiddleJob : IJob
    {      
            private readonly IServiceProvider _serviceProvider;
            public QuartzMiddleJob(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }
            public async Task Execute(IJobExecutionContext context)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jobType = context.JobDetail.JobType;
                    var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;
                    await job.Execute(context);
                }
            }
       
    }
}
