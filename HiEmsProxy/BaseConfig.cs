using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy
{
   public   class BaseConfig
    {
        public int AddressInterval { get; set; } //modbus任务合并地址间隔10
        public int ModbusMaxCount { get; set; } //modbus读取最大长度125
        public string SignalRUrl { get; set; }//Signal address
        public int UploadInterval { get; set; } //刷新snapshop数据间隔5s
        public string LocalIp{ get; set; } //采集器ip
        public string LocalId { get; set; } //采集器iD
        public string Location { get; set; } //采集器Location
        public string Name { get; set; } //采集器Name
    }
}
