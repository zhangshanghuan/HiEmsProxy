using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer.Model
{
    public class ResultLib
    {
        public string Result { get; set; }
        public string Value { get; set; }
        public byte[] ValueByteArray { get; set; }
        public string Router { get; set; }
        public string RW { get; set; }
        public string attach { get; set; }
    }
}
