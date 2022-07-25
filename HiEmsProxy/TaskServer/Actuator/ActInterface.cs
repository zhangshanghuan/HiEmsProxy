using System.Collections.Concurrent;
using System.Collections.Generic;
using HiEmsProxy.TaskServer.Model;

namespace HiEmsProxy.TaskServer.Actuator
{
    public interface ActInterface
    {
        public List<Tasklib> _TaskList { get; set; }
        public BlockingCollection<Tasklib> _ExcuteBlockingCollection { get; set; }
        public void Main();       
    }
}
