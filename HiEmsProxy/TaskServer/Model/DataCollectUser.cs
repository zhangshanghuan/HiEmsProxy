using System;
using System.Collections.Generic;
using System.Text;

namespace HiEmsProxy.TaskServer.Model
{
    public class DataCollectUser
    {
        /// <summary>
        /// 客户端连接Id
        /// </summary>
        public string ConnnectionId { get; set; }
        public string Name { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.Now;
        public string LocalIP { get; set; }
        public int LocalId { get; set; }     //采集执行ID
        public string Location { get; set; }       
    }
}
