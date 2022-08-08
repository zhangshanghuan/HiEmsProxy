using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer.Actuator
{
    public class ConStateModel
    {
        public int id { get; set; }
        public string description { get; set; }  
    }
    public  class common
    {

        public void DeviceConState(int id,string description)
        {
            if (DelegateLib.SignalDevConDelegate != null)
            {
                ConStateModel _ConStateModel = new ConStateModel()
                {
                    id = id,
                    description = description
                };
                string result = JsonCommon.SerializeDataContractJson<ConStateModel>(_ConStateModel);
                DelegateLib.SignalDevConDelegate(result);
            }
        }
    }
}
