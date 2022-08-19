using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
using HiEmsProxy.Quartz;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer
{
    /// <summary>
    /// 策略相关类
    /// </summary>
    public class StrategyExecute
    {
        private StrategyExecute()
        {
            DelegateLib.StrategyDelegate += ExecuteMain;
            DelegateLib.StrategyTriggerDelegate += Trigger;
        }
        //单例模式只初始化一次
        private static StrategyExecute instance = null;
        public static StrategyExecute GetInstance()
        {
            if (instance == null)
            {
                instance = new StrategyExecute();             
            }
            return instance;
        }


        public void ExecuteMain(Tasklib _Tasklib)
        {
            if (_Tasklib != null) Console.WriteLine("策略远端执行结果:" + _Tasklib.Value);
        }
        //策略触发事件（snapshot刷新时触发检查是否有策略需要执行）
        public void Trigger(string json)
        {            
            bool res = GetValue("bms_stack_soc", "","1",json);
        }



        //执行策略中的任务
        public void Excute(Tasklib tasklib)
        {
            tasklib.TaskType = 2;//策略相关任务类型是2
            if (DelegateLib.StrategyExecuteDelegate!=null)
            {
                DelegateLib.StrategyExecuteDelegate(tasklib);
            }
        }
        //获取Snapshot采集值
        public bool GetValue(string dict,string maxvalue, string minvalue, string json,int gl1=0,int gl2=0,int gl3=0,int gl4=0)
        {
            try
            {
                List<DataCollectHubVo> _listDataCollectHubVo = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(json);
                if (_listDataCollectHubVo != null)
                {
                    for (int i = 0; i < _listDataCollectHubVo.Count; i++)
                    {
                        DataCollectHubVo _dataCollectHubVo = _listDataCollectHubVo[i];
                        List<HiemsDeviceDataDtoForHub> _HiemsDeviceDataDtoForHub = _dataCollectHubVo.Data;
                        if (_HiemsDeviceDataDtoForHub != null)
                        {
                            for (int j = 0; j < _HiemsDeviceDataDtoForHub.Count; j++)
                            {
                                if (_HiemsDeviceDataDtoForHub[j].Dict!=null&&_HiemsDeviceDataDtoForHub[j].Dict.Trim() == dict)
                                {
                                    if (gl1 != 0 && _HiemsDeviceDataDtoForHub[j].Gl1 != gl1) continue;
                                    if (gl2 != 0 && _HiemsDeviceDataDtoForHub[j].Gl2 != gl2) continue;
                                    if (gl3 != 0 && _HiemsDeviceDataDtoForHub[j].Gl3 != gl3) continue;
                                    if (gl4 != 0 && _HiemsDeviceDataDtoForHub[j].Gl4 != gl4) continue;
                                    string value = _HiemsDeviceDataDtoForHub[i].Value;
                                    if (value != "" && value != null)
                                    {
                                        string[] Values = value.Split(',');
                                        for (int k= 0; k < Values.Length; k++)
                                        {
                                            if (CheckResult(maxvalue, minvalue, Values[k]))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public string GetValue(string dict, string json, int gl1 = 0, int gl2 = 0, int gl3 = 0, int gl4 = 0)
        {
            try
            {
                List<DataCollectHubVo> _listDataCollectHubVo = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(json);
                if (_listDataCollectHubVo != null)
                {
                    for (int i = 0; i < _listDataCollectHubVo.Count; i++)
                    {
                        DataCollectHubVo _dataCollectHubVo = _listDataCollectHubVo[i];
                        List<HiemsDeviceDataDtoForHub> _HiemsDeviceDataDtoForHub = _dataCollectHubVo.Data;
                        if (_HiemsDeviceDataDtoForHub != null)
                        {
                            for (int j = 0; j < _HiemsDeviceDataDtoForHub.Count; j++)
                            {
                                if (_HiemsDeviceDataDtoForHub[j].Dict != null && _HiemsDeviceDataDtoForHub[j].Dict.Trim() == dict)
                                {
                                    if (gl1 != 0 && _HiemsDeviceDataDtoForHub[j].Gl1 != gl1) continue;
                                    if (gl2 != 0 && _HiemsDeviceDataDtoForHub[j].Gl2 != gl2) continue;
                                    if (gl3 != 0 && _HiemsDeviceDataDtoForHub[j].Gl3 != gl3) continue;
                                    if (gl4 != 0 && _HiemsDeviceDataDtoForHub[j].Gl4 != gl4) continue;
                                    return _HiemsDeviceDataDtoForHub[i].Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        //值进行比较大小
        private static bool CheckResult(string max, string min, string values)
        {
            if (values == "" || values == null) return false;
            if (max == null) max = "";
            if (min == null) min = "";
            try
            {
                if (max == "" && min == "")
                {
                    if (values == ""|| values==null) return false;
                    return true;
                }
                else if (max == "" && min != "")
                {                 
                    if (!values.Trim().Equals(min.Trim()))
                    {
                        return false;
                    }
                    return true;
                }
                else if (max != "" && min != "")
                {                  
                    if (Convert.ToDouble(min.Trim()) <= Convert.ToDouble(values.Trim()) && Convert.ToDouble(values.Trim()) <= Convert.ToDouble(max.Trim()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {              
            }
            return false;
        }
    }
}
