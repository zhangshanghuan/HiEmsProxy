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
using HiEMS.Model.Models;
using Newtonsoft.Json;
using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
using HiEmsProxy.Quartz;

namespace HiEmsProxy.TaskServer
{
    public class HiemsMain
    {      
        RemoteExecute _RemoteExecute = new RemoteExecute();
        TaskDistrib _TaskDistrib;
        SignalClien _SignalClien = new SignalClien();
        UpLoadLib _UpLoadLib;
        public HiemsMain()
        {
            DelegateLib.SignalDevicePropDelegate += SignalDevicePropDelegate;
            DelegateLib.SignalRemoteDelegate += SignalRemoteDelegate;
        }
    
        public async void Start()
        {
            bool res = await _SignalClien.Connect();
            if (res) res = await _SignalClien.InitMain();
            Console.Write("SignalClien init "+(res ? "OK " : "NG ")+DateTime.Now.ToString()+"\r\n");
            _UpLoadLib = new UpLoadLib(_SignalClien);
        }
        //远端执行任务委托
        private void SignalRemoteDelegate(string json)
        {
            Root _Root = JsonConvert.DeserializeObject<Root>(json);
            if (_Root != null && _Root.code == 200)
            {
                List<Tasklib> _List = AnalyseJsonToTask(_Root,1);
                if (_List!=null&& _List.Count>0) InsertTask(_List);
            }
        }
        //采集任务委托
        public void SignalDevicePropDelegate(string json)
        {
            Root _Root = JsonConvert.DeserializeObject<Root>(json);
            if (_Root != null && _Root.code == 200)
            {
                //创建缓存数据
                List<DataCollectHubVo> cachedata = CreatResultJson(JsonConvert.SerializeObject(_Root.data));
                _UpLoadLib._CacheList = cachedata;
                //将任务分解提交给任务分发器
                List<Tasklib> tasklibs = AnalyseJsonToTask(_Root, 0);
                if (tasklibs != null && tasklibs.Count != 0)
                {
                    JsonCommon.SourceSnapshot = json;
                    _TaskDistrib = new TaskDistrib();
                    _TaskDistrib.DisToExecute(tasklibs);
                }
            }
        }




        //插入任务
        public void InsertTask(List<Tasklib> _TaskLib)
        {
            foreach (var item in _TaskLib)
            {
                Console.Write($"远端执行任务  {item.DeviceProperty.Describe}\r\n");
                _TaskDistrib.Insert(item);
            }           
        }
        /// <summary>
        /// 解析json 将每条json任务加入_ListTaskLib
        /// </summary>
        /// <param name="_Root"></param>
        /// <param name="tasktype">任务类型 0：采集任务  1：远端执行任务</param>
        /// <returns></returns>
        public List<Tasklib> AnalyseJsonToTask(Root _Root,int tasktype)
        {
            List<Tasklib> _List = new List<Tasklib>();          
            if (_Root!=null&& _Root.code == 200)
            {
                List<DataCollectHubVo> _list = _Root.data;               
                if (_list == null) return _List;
                for (int i = 0; i < _list.Count; i++)
                {
                    HiemsDeviceProtocol _DeviceProtocol = _list[i].Protocol;
                    List<HiemsDevicePropertyDto> _ListDeviceProperty = _list[i].Property;
                    if (_ListDeviceProperty == null) continue;             
                    foreach (HiemsDevicePropertyDto _DevicePropertyDto in _ListDeviceProperty)
                    {
                        Tasklib _TaskLib = new()
                        {
                            DeviceProtocol = _DeviceProtocol,
                            DeviceProperty = _DevicePropertyDto,
                            label = _list[i].Label,
                            TaskType = tasktype,                            
                        };                     
                        _TaskLib.infoId = _list[i].InfoId;
                        _TaskLib.deviceId = (int)_list[i].DeviceId;
                        _TaskLib.localId = _list[i].LocalId;
                        if (_DevicePropertyDto.StartAddr != null) _TaskLib.DeviceProperty.StartAddress = int.Parse(_DevicePropertyDto.StartAddr);
                        _TaskLib.Router = $"{_list[i].InfoId}+{_DevicePropertyDto.Id}+[{_DevicePropertyDto.DataIdx}]" ;
                        _List.Add(_TaskLib);
                    }
                }
            }
            return _List;
        }

        //生成一份结果的对象 CacheJson.json
        public List<DataCollectHubVo>  CreatResultJson(string json)
        {
            List<DataCollectHubVo> _listDataCollectHubVo;
            try
            {
                if (JsonCommon.CacheSnapshot!="")
                {
                    _listDataCollectHubVo = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(JsonCommon.CacheSnapshot);
                }
                else
                {
                    _listDataCollectHubVo = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(json);
                    for (int i = 0; i < _listDataCollectHubVo.Count; i++)
                    {
                        List<HiemsDevicePropertyDto> listProperty = _listDataCollectHubVo[i].Property;
                        if (listProperty == null|| listProperty.Count==0) continue;
                        _listDataCollectHubVo[i].Data = new List<HiemsDeviceDataDto>();
                        for (int j = 0; j < listProperty.Count; j++)
                        {
                            HiemsDeviceDataDto _HiemsDeviceDataDto = new HiemsDeviceDataDto()
                            {
                                Describe = listProperty[j].Describe,
                                PropertyId = listProperty[j].Id,
                                Name = listProperty[j].Name,
                                InfoId = _listDataCollectHubVo[i].InfoId,
                                LocalId = _listDataCollectHubVo[i].LocalId,
                                RwType = listProperty[j].RwType,
                                DataIdx = listProperty[j].DataIdx,
                                Code= listProperty[j].Code,
                            };
                          //  if (listProperty[j].Type.ToUpper() == "ALARM") _HiemsDeviceDataDto.Value = "False";
                            _listDataCollectHubVo[i].Data.Add(_HiemsDeviceDataDto);
                            //_listDataCollectHubVo[i].Property = null;
                            //_listDataCollectHubVo[i].Protocol = null;
                        }
                    }
                    JsonCommon.CacheSnapshot = JsonConvert.SerializeObject(_listDataCollectHubVo);
                }
                return _listDataCollectHubVo;
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        //清除缓存
        public void ClearCacheJson()
        {
            if (File.Exists(JsonCommon. CachePath))
            {
                JsonCommon.CacheSnapshot = "";
                File.Delete(JsonCommon.CachePath);
            }
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
