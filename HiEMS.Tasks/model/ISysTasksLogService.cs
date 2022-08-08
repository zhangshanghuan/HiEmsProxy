using System.Threading.Tasks;
using HiEMS.Model.System;
using HiEMS.Repository;

namespace HiEMS.Service.System.IService
{
    public interface ISysTasksLogService : IBaseService<SysTasksLog>
    {
        /// <summary>
        /// 记录任务执行日志
        /// </summary>
        /// <returns></returns>
        //public int AddTaskLog(string jobId);
        Task<SysTasksLog> AddTaskLog(string jobId, SysTasksLog tasksLog);
    }
}
