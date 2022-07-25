using HithiumEmsPlatForm.BLL.TaskServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer.Model
{
    public static class JsonCommon
    {
        public static string Str_json = File.ReadAllText("D:\\1.config");
        public static string Str_json1 = File.ReadAllText("D:\\2.config");
       
        public static string OldSnapshot = File.ReadAllText("D:\\1.config"); //平台加载生成的sanpshot
        public static string NewSnapshot = File.ReadAllText("D:\\1.config");//实时的
        public static string CalSnapshot = "";//差异值
        //比较两个差异值发给网关，然后把NewSnapShot覆盖OldSnapShot


        static object _obj = new object();
        //通过路由设置json值
        public static void SetJsonStrKey(string router, string key, string value, ref string json)
        {
            lock (_obj)
            {
                try
                {
                    string[] listrouter = router.Split('.');
                    if (listrouter.Length < 3) return;
                    JObject jo = JObject.Parse(json);
                    JObject joCom = (JObject)jo.GetValue("deviceInfo")[listrouter[listrouter.Length - 3]];
                    JArray resultArray = joCom.GetValue(listrouter[listrouter.Length - 2]) != null ? (JArray)joCom.GetValue(listrouter[listrouter.Length - 2]) : null;
                    if (resultArray == null) return;
                    for (int i = 0; i < resultArray.Count; i++)
                    {
                        if (resultArray[i][key] == null || resultArray[i]["Name"].ToString() != listrouter[listrouter.Length - 1])
                        {
                            continue;
                        }
                        resultArray[i][key] = value;
                        break;
                    }
                    json = jo.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        //通过路由获取json值     //BMS.AlarmInfo.alarm
        public static string GetJsonStrKey(string router, string key, string json)
        {
            lock (_obj)
            {
                try
                {
                    string[] listrouter = router.Split('.');
                    if (listrouter.Length < 3) return "";
                    JObject jo = JObject.Parse(json);
                    JObject joCom = (JObject)jo.GetValue("deviceInfo")[listrouter[listrouter.Length - 3]];
                    JArray resultArray = (JArray)joCom.GetValue(listrouter[listrouter.Length - 2]);
                    for (int i = 0; i < resultArray.Count; i++)
                    {
                        if (resultArray[i][key] == null || resultArray[i]["name"].ToString() != listrouter[listrouter.Length - 1])
                        {
                            continue;
                        }
                        return resultArray[i][key].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return "";
            }
        }


        public static void SetJsonStrKey1(string router, string key, string value, ref string json)
        {
            lock (_obj)
            {
                try
                {
                    string[] listrouter = router.Split('.');
                    JObject jo = JObject.Parse(json);
                    JObject joCom = null;
                    for (int i = 0; i < listrouter.Length; i++)
                    {
                        joCom = (JObject)getJObject(jo, listrouter[i]);
                        if (joCom != null) 
                        {
                            var dd = (JObject)getJObject(joCom, "config");
                            if (dd != null)
                            {
                                JObject data = (JObject)getJObject(joCom, listrouter[i + 1]);
                                data[listrouter[i + 2]][key] = value;
                                json = joCom.ToString();
                                return;
                            }
                            //else
                            //{
                            //    jo = joCom;
                            //}
                        }
                    }                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetJsonStrKey1(string router, string key, string json)
        {
            lock (_obj)
            {
                try
                {
                    string[] listrouter = router.Split('.');
                    JObject jo = JObject.Parse(json);
                    JObject joCom = null;
                    for (int i = 0; i < listrouter.Length; i++)
                    {
                        joCom = (JObject)getJObject(jo, listrouter[i]);
                        if (joCom != null)
                        {
                            var dd = (JObject)getJObject(joCom, "config");
                            if (dd != null)
                            {
                                JObject data = (JObject)getJObject(joCom, listrouter[i + 1]);
                                return data[listrouter[i + 2]][key].ToString() ;
                            }
                            else
                            {
                                jo = joCom;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return "";
            }
        }
        private static object getJObject(JObject joCom, string item)
        {
            return joCom[item];
        }





        //序列化
        public static string SerializeDataContractJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray());
                return szJson;
            }
        }
        //反序化
        public static T DeserializeDataContractJson<T>(string szJson)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
