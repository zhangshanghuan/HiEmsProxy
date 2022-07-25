using HithiumEmsPlatForm.BLL.TaskServer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using HiEmsProxy.TaskServer;
using ModbusLibNew;

namespace HiEmsProxy.TaskServer
{
    public class HiemsMain
    {
        string json = JsonCommon.Str_json;
        string json1 = JsonCommon.Str_json1;
        UpLoadLib _UpLoadLib = new UpLoadLib();
        RemoteExecute _RemoteExecute = new RemoteExecute();
        TaskDistrib _TaskDistrib;
        int i = 0;
        public void Start()
        {
             List<Tasklib> _List  =  AnalyseJsonToTask(json);
            _TaskDistrib = new TaskDistrib();            
            _TaskDistrib.DisToExecute(_List);        

            Task.Run(() =>
            {
                //while (true)
                //{
                //    i++;
                //    Thread.Sleep(5000);
                //    Console.WriteLine("更新json");
                //    if (i % 2 == 0)
                //    {
                //        RefreshJson(json1);
                //    }
                //    else
                //    {
                //        RefreshJson(json);
                //    }
                //}

                while (true)
                {
                    Thread.Sleep(3000);
                    Tasklib _TaskLib = new Tasklib();
                    _TaskLib.Router = "PCS.AlarmInfo.alarm";
                    _TaskLib.DeviceProtocol = GetConfig(json, "PCS");
                    _TaskLib.DeviceProperty = new HiemsDeviceProperty()
                    {
                        Describe = "掺入任务1",
                        Function = "ReadDiscreteInputs",
                        StartAddress = 0,
                        Length = 10,
                        RwType = "R",
                        SlaveId = 1,
                        Level = 1,           
                        Value = "1 1 1",
                        RefreshRate = 1,
                    };
                    InsertTask(_TaskLib);
                }
            });
        }

        //更新json
        string OldJson = string.Empty;
        public void RefreshJson(string json)
        {
            if (json == OldJson) return;
            DelegateLib.manual.Set();
             List<Tasklib> _List = AnalyseJsonToTask(json);                      
            _TaskDistrib.UpdateAllTask(_List);          
            DelegateLib.manual.Reset();
        }


        //插入任务
        public void InsertTask(Tasklib _TaskLib)
        {
            Console.Write($"插入新的任务  {_TaskLib.DeviceProperty.Describe}\r\n");
            _TaskDistrib.Insert(_TaskLib);
        }


        //解析json 将每条json任务加入_ListTaskLib
        private List<Tasklib> _ListTaskLib = new List<Tasklib>();
        public List<Tasklib> AnalyseJsonToTask(string json)
        {
            _ListTaskLib.Clear();
            HiemsDeviceProtocol _config =null;
            JObject jo = JObject.Parse(json);
            var _DeviceInfo = jo["deviceInfo"].ToString();
            var list = JObject.Parse(_DeviceInfo.ToString()).Properties().ToList();
            foreach (var module in list)
            {
                string Modulejson = jo["deviceInfo"][module.Name].ToString();
                JObject Modulejo = JObject.Parse(Modulejson);
                JToken jToken = JToken.Parse(Modulejson);
                var Modulelist = jToken.Select(x => x.ToObject<JProperty>().Name).ToList();
                foreach (var item in Modulelist)
                {
                    if (item == "config") {
                         _config = JsonCommon.DeserializeDataContractJson<HiemsDeviceProtocol>(Modulejo[item].ToString());
                        continue;
                    }          
                    _ListTaskLib.AddRange(ForeachNode($"deviceInfo.{module.Name}.{item}", Modulejo[item].ToString(), _config));
                }
            }
            return _ListTaskLib;
        }
        private List<Tasklib> ForeachNode(string router, string json, HiemsDeviceProtocol _config)
        {
            List<Tasklib> _List = new List<Tasklib>();
            JToken jToken = JToken.Parse(json);
            JObject jo = JObject.Parse(json);
            var list = jToken.Select(x => x.ToObject<JProperty>().Name).ToList();
            foreach (var item in list)
            {
                for (int i = 0; i < 1; i++)
                {
                    HiemsDeviceProperty _DeviceData = JsonCommon.DeserializeDataContractJson<HiemsDeviceProperty>(jo[item].ToString());
                    _DeviceData.StartAddress = i + 1;                   
                    if (_DeviceData != null)
                    {
                        Tasklib _TaskLib = new()
                        {
                            DeviceProtocol = _config,
                            DeviceProperty = _DeviceData,
                            Router = router + "." + item+(i+1).ToString(),
                        };
                        _List.Add(_TaskLib);
                    }
                }
            }
            return _List;
        }
        //获取模块的config
        private HiemsDeviceProtocol GetConfig(string json,string modulename)
        {
            JObject jo = JObject.Parse(json);
            string Modulejson = jo["deviceInfo"][modulename].ToString();
            JObject Modulejo = JObject.Parse(Modulejson);
            HiemsDeviceProtocol _config = JsonCommon.DeserializeDataContractJson<HiemsDeviceProtocol>(Modulejo["config"].ToString());
            return _config;
        }


        #region 动态库反射获取对应的方法名
        //object obj = null;
        //Type type = null;
        //public static string InitJson = "{\"_TestLightIndex\":1}";
        //object[] parasObj;
        //System.Collections.Hashtable hashtable = new System.Collections.Hashtable();
        ///// <summary>
        ///// 反射执行
        ///// </summary>
        ///// <param name="classname">类名</param>
        ///// <param name="path">动态库路径（运行路径下之后的路径）</param>
        ///// <param name="MethodName">方法名</param>
        ///// <returns></returns>
        //public string Assembly(string path, string MethodName, object param, string classname = "DeviceLibrary")
        //{
        //    try
        //    {
                 
        //        parasObj = new object[] { InitJson };
        //        int index = path.LastIndexOf('\\');
        //        string dllname = path.Substring(index + 1);
        //        string typename = dllname + "." + classname;
        //        path = Path.Combine(GetAssemblyPath(), path + ".dll");
        //        if (!File.Exists(path)) return "";
        //        //参数
        //        if (param.ToString() != "") parasObj = new object[] { param };
        //        if (!hashtable.Contains(dllname))
        //        {
        //            Assembly assembly = System.Reflection.Assembly.LoadFrom(path);
        //            Type type = assembly.GetType(typename);
        //            object obj = Activator.CreateInstance(type);
        //            hashtable.Add(dllname, obj);
        //        }
        //        MethodInfo method = type.GetMethod(MethodName);
        //        dynamic res = method.Invoke(obj, parasObj);
        //        InitJson = Convert.ToString(parasObj[0]);
        //        if (res is Task)
        //        {
        //            res.Wait();
        //            string ddd = res.Result;
        //            return res.Result;
        //        }
        //        return res;
        //    }
        //    catch (Exception EX)
        //    {
        //        return "";
        //    }
        //}

        //private static string GetAssemblyPath()
        //{
        //    string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
        //    _CodeBase = _CodeBase[8..];    // 8是file:// 的长度
        //    string[] arrSection = _CodeBase.Split(new char[] { '/' });

        //    string _FolderPath = "";
        //    for (int i = 0; i < arrSection.Length - 1; i++)
        //    {
        //        _FolderPath += arrSection[i] + "/";
        //    }

        //    return _FolderPath;
        //}
        #endregion
    }
}
