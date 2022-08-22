using System;
using System.Collections.Generic;
using System.Text;

namespace HiEmsProxy.TaskServer.Model
{
    public class DataCollectUser
    {      
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public DateTime LoginTime { get; set; }
        public string LocalIP { get; set; }
        public int LocalId { get; set; }     //采集执行ID
        public string Location { get; set; }
        public string UserType { get; set; }  //用户类型   COLLECT   SHOW
        public string UserId { get; set; } //用户id
    }
}
