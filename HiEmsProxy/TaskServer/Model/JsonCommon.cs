using HiEMS.Model.Business.Vo;
using HiEMS.Model.Dto;
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
    public class JsonFileHelper
    {
        private string _path;//根目录的相对途径   
        /// <param name="jsonName">根目录的相对途径包含文件名</param>
        public JsonFileHelper(string jsonName)
        {
            if (!jsonName.EndsWith(".json"))
            {
                _path = $"{jsonName}.json";
            }
            else
            {
                _path = jsonName;
            }
            if (!File.Exists(_path))
            {//不存在 创建一个json文件
                File.WriteAllText(_path, "{}");
            }
        }
        public JsonFileHelper()
        { }

        /// <summary>
        /// 读取Json返回实体对象
        /// </summary>
        /// <returns></returns>
        public T Read<T>() => Read<T>("");

        /// <summary>
        /// 根据节点读取Json返回实体对象
        /// </summary>
        /// <param name="section">根节点</param>
        /// <returns></returns>
        public T Read<T>(string section)
        {
            try
            {
                using (StreamReader file = new StreamReader(_path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JToken secJt = JToken.ReadFrom(reader);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        string[] nodes = section.Split('.');
                        foreach (string node in nodes)
                        {
                            secJt = secJt[node];
                        }
                        if (secJt != null)
                        {
                            return JsonConvert.DeserializeObject<T>(secJt.ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<T>(secJt.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return default(T);
        }

        /// <summary>
        /// 读取Json返回集合
        /// </summary>
        /// <returns></returns>
        public List<T> ReadList<T>() => ReadList<T>("");

        /// <summary>
        /// 根据节点读取Json返回集合
        /// </summary>
        /// <param name="section">根节点</param>
        /// <returns></returns>
        public List<T> ReadList<T>(string section)
        {
            try
            {
                using (StreamReader file = new StreamReader(_path))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JToken secJt = JToken.ReadFrom(reader);
                        if (!string.IsNullOrWhiteSpace(section))
                        {
                            string[] nodes = section.Split('.');
                            foreach (string node in nodes)
                            {
                                secJt = secJt[node];
                            }
                            if (secJt != null)
                            {
                                return JsonConvert.DeserializeObject<List<T>>(secJt.ToString());
                            }
                        }
                        else
                        {
                            return JsonConvert.DeserializeObject<List<T>>(section.ToString());
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return default(List<T>);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T">自定义对象</typeparam>
        /// <param name="t"></param>
        public void Write<T>(T t) => Write("", t);

        /// <summary>
        /// 写入指定section文件
        /// </summary>
        /// <typeparam name="T">自定义对象</typeparam>
        /// <param name="t"></param>
        public void Write<T>(string section, T t)
        {
            try
            {
                JObject jObj;
                string json = JsonConvert.SerializeObject(t);
                if (string.IsNullOrWhiteSpace(section))
                {
                    jObj = JObject.Parse(json);
                }
                else
                {
                    using (StreamReader file = new StreamReader(_path))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        try
                        {
                            jObj = (JObject)JToken.ReadFrom(reader);
                            jObj[section] = JObject.Parse(json);
                        }
                        catch (Exception ex)
                        {
                            jObj = JObject.Parse(json);
                        }
                    }
                }
                using (StreamWriter writer = new StreamWriter(_path))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        jObj.WriteTo(jsonWriter);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 读取节点内容
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        /// <returns></returns>
        public string Read(string section)
        {
            try
            {
                JToken jObj;
                using (StreamReader file = new StreamReader(_path))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        jObj = JToken.ReadFrom(reader);
                        string[] nodes = section.Split('.');
                        JToken tempToken = jObj;
                        if (nodes != null && nodes.Length > 0)
                        {
                            foreach (string node in nodes)
                            {
                                if (node != null)
                                {
                                    if (node.StartsWith("[") && node.EndsWith("]"))
                                    {
                                        int tempi = 0;
                                        if (int.TryParse(node, out tempi))
                                        {
                                            tempToken = tempToken[tempi];
                                        }
                                    }
                                    else
                                    {
                                        tempToken = tempToken[node];
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (tempToken != null)
                        {
                            return tempToken.ToString();
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return "";
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        public void Remove(string section)
        {
            JTokenHander(section, true, "", false);
        }
        /// <summary>
        /// 修改指定节点内容
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        /// <param name="value">修改节点的内容</param>
        /// <param name="isNum">修改内容类型 数据:true  字符串：false</param>
        public void Update(string section, string value, bool isNum)
        {
            JTokenHander(section, false, value, isNum);
        }

        /// <summary>
        /// 删除或修改 单个节点
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        /// <param name="isRemove">是删除还是修改</param>
        /// <param name="value">修改节点的内容</param>
        /// <param name="isNum">修改内容类型 数据:true  字符串：false</param>
        private void JTokenHander(string section, bool isRemove, string value, bool isNum)
        {
            try
            {
                JToken jObj;
                using (StreamReader file = new StreamReader(_path))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        jObj = JToken.ReadFrom(reader);
                        string[] nodes = section.Split('.');
                        JToken tempToken = jObj;
                        if (nodes != null && nodes.Length > 0)
                        {
                            foreach (string node in nodes)
                            {
                                if (node != null)
                                {
                                    if (node.StartsWith("[") && node.EndsWith("]"))
                                    {
                                        int tempi = 0;
                                        if (int.TryParse(node.Trim('[').Trim(']'), out tempi))
                                        {
                                            tempToken = tempToken[tempi];
                                        }
                                    }
                                    else
                                    {
                                        tempToken = tempToken[node];
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (tempToken != null)
                        {
                            if (isRemove)
                            {
                                tempToken.Parent.Remove();
                            }
                            else
                            {
                                JToken token;
                                if (isNum)
                                {
                                    if (value.Contains("."))
                                    {
                                        double tempd;
                                        double.TryParse(value, out tempd);
                                        token = tempd;
                                    }
                                    else
                                    {
                                        int tempi = 0;
                                        int.TryParse(value, out tempi);
                                        token = tempi;
                                    }
                                }
                                else
                                {
                                    token = value;
                                }
                                tempToken.Replace(token);//改
                            }
                        }
                    }
                }
                using (StreamWriter writer = new StreamWriter(_path))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        jObj.WriteTo(jsonWriter);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }



        #region @Author upfun 20220728
        /// <summary>
        /// 读取节点内容
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        /// <returns></returns>
        public string Read(string section, string json)
        {
            try
            {
                JToken jObj;
                jObj = JToken.Parse(json);
                string[] nodes = section.Split('.');
                JToken tempToken = jObj;
                if (nodes != null && nodes.Length > 0)
                {
                    foreach (string node in nodes)
                    {
                        if (node != null)
                        {
                            if (node.StartsWith("[") && node.EndsWith("]"))
                            {
                                int tempi = 0;
                                if (int.TryParse(node, out tempi))
                                {
                                    tempToken = tempToken[tempi];
                                }
                            }
                            else
                            {
                                tempToken = tempToken[node];
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (tempToken != null)
                {
                    return tempToken.ToString();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "";
        }

        /// <summary>
        /// 根据节点读取Json返回集合
        /// </summary>
        /// <param name="section">根节点</param>
        /// <returns></returns>
        public List<T> ReadList<T>(string section, string json)
        {
            try
            {
                JToken secJt = JToken.Parse(json);
                if (!string.IsNullOrWhiteSpace(section))
                {
                    string[] nodes = section.Split('.');
                    foreach (string node in nodes)
                    {
                        secJt = secJt[node];
                    }
                    if (secJt != null)
                    {
                        return JsonConvert.DeserializeObject<List<T>>(secJt.ToString());
                    }
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<T>>(section.ToString());
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return default(List<T>);
        }

        /// <summary>
        /// 根据节点读取Json返回实体对象
        /// </summary>
        /// <param name="section">根节点</param>
        /// <returns></returns>
        public T Read<T>(string section, string json)
        {
            try
            {
                JToken secJt = JToken.Parse(json);
                if (!string.IsNullOrWhiteSpace(section))
                {
                    string[] nodes = section.Split('.');
                    foreach (string node in nodes)
                    {
                        secJt = secJt[node];
                    }
                    if (secJt != null)
                    {
                        return JsonConvert.DeserializeObject<T>(secJt.ToString());
                    }
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(secJt.ToString());
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return default(T);
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        public void Remove(string section, ref string json)
        {
            JTokenHande_upfun(section, true, "", false, ref json);
        }
        /// <summary>
        /// 修改指定节点内容
        /// </summary>
        /// <param name="section">节点途径 格式为A.B.C  如果节点带数组格式  A.[0].C 数组用中括号</param>
        /// <param name="value">修改节点的内容</param>
        /// <param name="isNum">修改内容类型 数据:true  字符串：false</param>
        public void Update(string section, string value, bool isNum, ref string json)
        {
            JTokenHande_upfun(section, false, value, isNum, ref json);
        }

        /// <summary>
        /// 写入指定section文件
        /// </summary>
        /// <typeparam name="T">自定义对象</typeparam>
        /// <param name="t"></param>
        public void Write<T>(string section, T t, ref string json)
        {
            try
            {
                JObject jObj;
                string _json = JsonConvert.SerializeObject(t);
                if (string.IsNullOrWhiteSpace(section))
                {
                    jObj = JObject.Parse(_json);
                }
                else
                {
                    try
                    {
                        jObj = JObject.Parse(json);
                        jObj[section] = JObject.Parse(_json);
                    }
                    catch (Exception ex)
                    {
                        jObj = JObject.Parse(json);
                    }
                }
                json = jObj.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 节点下添加一个属性
        /// </summary>
        /// <param name="section">节点</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="json"></param>
        public void Add(string section, string key, string value, ref string json)
        {
            JToken jObj;
            try
            {
                jObj = JToken.Parse(json);
                string[] nodes = section.Split('.');
                JToken tempToken = jObj;
                if (nodes != null && nodes.Length > 0)
                {
                    foreach (string node in nodes)
                    {
                        if (node != null)
                        {
                            if (node.StartsWith("[") && node.EndsWith("]"))
                            {
                                int tempi = 0;
                                if (int.TryParse(node.Trim('[').Trim(']'), out tempi))
                                {
                                    tempToken = tempToken[tempi];
                                }
                            }
                            else
                            {
                                tempToken = tempToken[node];
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (tempToken != null)
                {
                    tempToken[key] = value;
                    json = ((JObject)jObj).ToString();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void JTokenHande_upfun(string section, bool isRemove, string value, bool isNum, ref string json)
        {
            JToken jObj;
            try
            {
                jObj = JToken.Parse(json);
                string[] nodes = section.Split('.');
                JToken tempToken = jObj;
                if (nodes != null && nodes.Length > 0)
                {
                    foreach (string node in nodes)
                    {
                        if (node != null)
                        {
                            if (node.StartsWith("[") && node.EndsWith("]"))
                            {
                                int tempi = 0;
                                if (int.TryParse(node.Trim('[').Trim(']'), out tempi))
                                {
                                    tempToken = tempToken[tempi];
                                }
                            }
                            else
                            {
                                tempToken = tempToken[node];
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (tempToken != null)
                {
                    if (isRemove)
                    {
                        tempToken.Parent.Remove();
                    }
                    else
                    {
                        JToken token;
                        if (isNum)
                        {
                            if (value.Contains("."))
                            {
                                double tempd;
                                double.TryParse(value, out tempd);
                                token = tempd;
                            }
                            else
                            {
                                int tempi = 0;
                                int.TryParse(value, out tempi);
                                token = tempi;
                            }

                        }
                        else
                        {
                            token = value;
                        }
                        tempToken.Replace(token);//改
                    }
                    json = ((JObject)jObj).ToString();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
    public static class JsonCommon
    {
        static object _obj = new object();
        public  static string CachePath = Path.Combine(GetAssemblyPath(), "CacheJson.json");
        public static string SourceSnapshot = "";     
        public static string CacheSnapshot = "";
  
        //将缓存队列存入本地cachejson.json文件
        public static  void WriteCacheFile(string json)
        {
            File.WriteAllText(CachePath, json);
        }
        public static string ReadCacheFile()
        {
            if (File.Exists(CachePath))
            {
                return File.ReadAllText(CachePath);
            }
            return "[]";
        }



        //生成一份结果的对象 CacheJson.json
        public static List<DataCollectHubVo>  GetCacheJson()
        {
            List<DataCollectHubVo> _listDataCollectHubVo = new List<DataCollectHubVo>();
            if (!File.Exists(CachePath))
            {
                return _listDataCollectHubVo;
            }
            string json=  File.ReadAllText(CachePath);
            try
            {
                    _listDataCollectHubVo = JsonConvert.DeserializeObject<List<DataCollectHubVo>>(json);
                    for (int i = 0; i < _listDataCollectHubVo.Count; i++)
                    {
                        List<HiemsDevicePropertyDto> listProperty = _listDataCollectHubVo[i].Property;
                        if (listProperty == null || listProperty.Count == 0) continue;
                        _listDataCollectHubVo[i].Data = new List<HiemsDeviceDataDtoForHub>();
                        for (int j = 0; j < listProperty.Count; j++)
                        {
                            HiemsDeviceDataDtoForHub _HiemsDeviceDataDtoForHub = new HiemsDeviceDataDtoForHub()
                            {
                                Describe = listProperty[j].Describe,
                                PropertyId = listProperty[j].Id,
                                Name = listProperty[j].Name,
                                InfoId = _listDataCollectHubVo[i].InfoId,
                                LocalId = _listDataCollectHubVo[i].LocalId,
                                RwType = listProperty[j].RwType,
                                Code = listProperty[j].Code,
                                Dict = listProperty[j].Dict,
                                Type = listProperty[j].Type,
                                Rank = listProperty[j].Rank,
                                Gl1 = listProperty[j].Gl1,
                                Gl2 = listProperty[j].Gl2,
                                Gl3 = listProperty[j].Gl3,
                                Gl4 = listProperty[j].Gl3,
                            };                        
                        }                 
                }
                return _listDataCollectHubVo;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        //清除缓存
        public static void ClearCacheJson()
        {
            if (File.Exists(JsonCommon.CachePath))
            {
                JsonCommon.CacheSnapshot = "";
                File.Delete(JsonCommon.CachePath);
            }
        }

        private static string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            _CodeBase = _CodeBase[8..];    // 8是file:// 的长度
            string[] arrSection = _CodeBase.Split(new char[] { '/' });
            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }
            return _FolderPath;
        } 
    }
}
