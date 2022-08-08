using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsk.Core.QuartzNet
{
    public class WeskyJobSchedule
    {
        public WeskyJobSchedule(Type jobType, string cronExpression, string iD)
        {
            this.JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            CronExpression = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
            ID = iD;
        }
        /// <summary>
        /// Job类型
        /// </summary>
        public Type JobType { get; private set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string CronExpression { get; private set; }
        /// <summary>
        /// Job状态
        /// </summary>
        public JobStatus JobStatu { get; set; } = JobStatus.Init;

        public string ID { get; set; }
    }

    /// <summary>
    /// 运行状态
    /// </summary>
    public enum JobStatus : byte
    {
        [Description("Initialization")]
        Init = 0,
        [Description("Running")]
        Running = 1,
        [Description("Scheduling")]
        Scheduling = 2,
        [Description("Stopped")]
        Stopped = 3,
    }
}
