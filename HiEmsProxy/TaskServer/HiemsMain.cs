using HithiumEmsPlatForm.BLL.TaskServer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;
using HiEMS.Model.Models;
using Newtonsoft.Json;
using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace HiEmsProxy.TaskServer
{
    public class HiemsMain
    {
        RemoteExecute _RemoteExecute = RemoteExecute.getInstance();
        StrategyExecute _StrategyExecute = StrategyExecute.GetInstance();
        SignalClien _SignalClien = SignalClien.getInstance();
        TaskDistrib _TaskDistrib  = new TaskDistrib();
        UpLoadLib _UpLoadLib = new UpLoadLib();
        public HiemsMain()
        {
            DelegateLib.SignalDevicePropDelegate += SignalDevicePropDelegate;
            DelegateLib.SignalRemoteDelegate += SignalRemoteDelegate;
            DelegateLib.StrategyExecuteDelegate += StrategyExecuteDelegate;
            //Task.Factory.StartNew(() => {
            //    Thread.Sleep(10000);
            //   string json= File.ReadAllText("D:\\1.json");
            //    SignalRemoteDelegate(json);
            //});
        }
    
        public async void Start()
        {
            bool res = await _SignalClien.Connect();
            if (res) res = await _SignalClien.InitMain();
            Console.Write("SignalClien init "+(res ? "OK " : "NG ")+DateTime.Now.ToString()+"\r\n");       
        }
   
        //远端执行任务委托
        private void SignalRemoteDelegate(string json)
        {
            Root _Root = JsonConvert.DeserializeObject<Root>(json);
            if (_Root != null && _Root.code == 200)
            {
                List<Tasklib> _List = AnalyseJsonToTask(_Root,1);
                if (_List!=null&& _List.Count>0) InsertTasks(_List);
            }
        }

        //采集任务委托  
        public void SignalDevicePropDelegate(string json)
        {
            Root _Root = JsonConvert.DeserializeObject<Root>(json);
            if (_Root != null && _Root.code == 200)
            {               
                _UpLoadLib._UploadList = JsonConvert.DeserializeObject<Root>(json).data;
                //将任务分解提交给任务分发器
                List<Tasklib> tasklibs = AnalyseJsonToTask(_Root, 0);
                if (tasklibs != null && tasklibs.Count != 0)
                {
                    JsonCommon.SourceSnapshot = json;                  
                    _TaskDistrib.DisToExecute(tasklibs);
                }
                _UpLoadLib._Timer.Enabled = true;
            }
        }

        //策略执行动作任务
        public void StrategyExecuteDelegate(Tasklib _Tasklib)
        {
            _TaskDistrib.Insert(_Tasklib);
        }

        //插入任务
        private void InsertTasks(List<Tasklib> _TaskLib)
        {
            foreach (var item in _TaskLib)
            {
                Console.Write($"远端执行任务  {item.DeviceProperty.Describe}\r\n");
                 if(_TaskDistrib!=null)  _TaskDistrib.Insert(item);
            }           
        }
        /// <summary>
        /// 解析json 将每条json任务加入_ListTaskLib
        /// </summary>
        /// <param name="_Root"></param>
        /// <param name="tasktype">任务类型 0：采集任务  1：远端执行任务</param>
        /// <returns></returns>
        public List<Tasklib> AnalyseJsonToTask(Root _Root, int tasktype)
        {
            List<Tasklib> _List = new List<Tasklib>();
            try
            {
                List<DataCollectHubVo> _list = _Root.data;
                if (_list == null) return _List;
                for (int i = 0; i < _list.Count; i++)
                {
                    List<HiemsDevicePropertyDto> _ListDeviceProperty = _list[i].Property;
                    if (_ListDeviceProperty == null) continue;
                    foreach (HiemsDevicePropertyDto _DevicePropertyDto in _ListDeviceProperty)
                    {
                        Tasklib _TaskLib = new()
                        {
                            DeviceProtocol = _list[i].Protocol,
                            Info = _list[i].Info,
                            DeviceProperty = _DevicePropertyDto,
                            Label = _list[i].Label,
                            TaskType = tasktype,
                            InfoId = _list[i].InfoId,
                            DeviceId = (int)_list[i].DeviceId,
                            LocalId = _list[i].LocalId,
                            Router = $"{_list[i].InfoId}+{_DevicePropertyDto.Id}"
                        };
                        _TaskLib.DeviceProperty.StartAddress = (int)_DevicePropertyDto.StartAddr;
                        _List.Add(_TaskLib);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return _List;
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

        private void Assembly1()
        {

            //加载程序集(dll文件地址)，使用Assembly类   
            Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "ClassLibrary1.dll");

            //获取类型，参数（名称空间+类）   
            Type type = assembly.GetType("ClassLibrary1.Class2");
            //创建该对象的实例，object类型，参数（名称空间+类）   
            object instance = assembly.CreateInstance("ClassLibrary1.Class2");

            //设置ClassLibrary1.dll中Show_Str方法中的参数类型，Type[]类型；如有多个参数可以追加多个   
            Type[] params_type = new Type[1];
            params_type[0] = Type.GetType("System.String");//System.Int32是整型
                                                           //设置ClassLibrary1.dll中Show_Str方法中的参数值；如有多个参数可以追加多个   
            Object[] params_obj = new Object[1];
            params_obj[0] = "lqwvje-Dll里面方法参数";

            //执行Show_Str方法   params_type方法的参数类型   instance对象实例 params_obj参数值
            object value = type.GetMethod("Show_Str", params_type).Invoke(instance, params_obj);
        }

        #endregion
    }
}
