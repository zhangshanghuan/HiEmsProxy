using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer.Models
{
    public class HiemsResultValue
    {
        public string Describe { get; set; }
        public int id { get; set; }
        public int DataIdx { get; set; }
        public string value { get; set; }
        public string result { get; set; }
        public string  rw { get; set; }
        public string time { get; set; }
    }
}
