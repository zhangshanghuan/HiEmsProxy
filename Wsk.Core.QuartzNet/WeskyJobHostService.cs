using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsk.Core.QuartzNet
{
    public class WeskyJobHostService : IHostedService
    {
           
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<WeskyJobSchedule> _jobSchedules;

        public WeskyJobHostService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<WeskyJobSchedule> jobSchedules)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            _jobSchedules = jobSchedules ?? throw new ArgumentNullException(nameof(jobSchedules));
        }

      
        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {         
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);              
                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                jobSchedule.JobStatu = JobStatus.Scheduling;
            }
            await Scheduler.Start(cancellationToken);
            foreach (var jobSchedule in _jobSchedules)
            {
                jobSchedule.JobStatu = JobStatus.Running;
            }        
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {          
            await Scheduler?.Shutdown(cancellationToken);
            foreach (var jobSchedule in _jobSchedules)
            {
                jobSchedule.JobStatu = JobStatus.Stopped;
            }
        }


        public  async Task StopAsync()
        { 
            await Scheduler?.Shutdown();
            foreach (var jobSchedule in _jobSchedules)
            {
                if (jobSchedule.ID=="1")
                {
                    jobSchedule.JobStatu = JobStatus.Stopped;
                }
                
            }
        }


        private static IJobDetail CreateJob(WeskyJobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)                
                .Build();
        }
        private static ITrigger CreateTrigger(WeskyJobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }
}
