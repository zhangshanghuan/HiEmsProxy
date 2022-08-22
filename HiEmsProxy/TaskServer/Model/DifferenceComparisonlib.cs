using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiEmsProxy.TaskServer.Model
{
    class DifferenceComparisonlib
    {

        /// <summary>
        /// 实体差异比较器
        /// </summary>
        /// <param name="source">源版本实体</param>
        /// <param name="current">当前版本实体</param>
        /// <returns>true 存在变更 false 未变更</returns>
        public static bool DifferenceComparison<T1, T2>(T1 source, T2 current, List<string> exclude = null)
        {
            Type t1 = source.GetType();
            Type t2 = current.GetType();
            PropertyInfo[] property1 = t1.GetProperties();
            //排除主键和基础字段
            //List<string> exclude = new List<string>() { "Id", "InsertTime", "UpdateTime", "DeleteTime", "Mark", "Version", "Code" };
            foreach (PropertyInfo p in property1)
            {
                string name = p.Name;
             //   if (exclude.Contains(name)) { continue; }
                string value1 = p.GetValue(source, null)?.ToString();
                string value2 = t2.GetProperty(name)?.GetValue(current, null)?.ToString();
                if (value1 != value2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 集合差异比较器,比较两个实体集合值是否一样
        /// </summary>
        /// <param name="source">源版本实体集合</param>
        /// <param name="current">当前版本实体集合</param>
        /// <returns>true 存在变更 false 未变更</returns>
        protected static bool DifferenceComparison<T1, T2>(List<T1> source, List<T2> current)
        {
            if (source.Count != current.Count) { return true; }
            for (int i = 0; i < source.Count; i++)
            {
                bool flag = DifferenceComparison(source[i], current[i]);
                if (flag) { return flag; }
            }
            return false;
        }
        /// <summary>
        /// 将实体2的值动态赋值给实体1(名称一样的属性进行赋值)
        /// </summary>
        /// <param name="model1">实体1</param>
        /// <param name="model2">实体2</param>
        /// <returns>赋值后的model1</returns>
        protected static T1 BindModelValue<T1, T2>(T1 model1, T2 model2) where T1 : class where T2 : class
        {
            Type t1 = model1.GetType();
            Type t2 = model2.GetType();
            PropertyInfo[] property2 = t2.GetProperties();
            //排除主键
            List<string> exclude = new List<string>() { "Id" };
            foreach (PropertyInfo p in property2)
            {
                if (exclude.Contains(p.Name)) { continue; }
                t1.GetProperty(p.Name)?.SetValue(model1, p.GetValue(model2, null));
            }
            return model1;
        }


        /// <summary>
        /// 比较两个对象是否有差异
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Result ComPare<T>(T t, T s)
        {
            Result result = new Result();
            var comparer = new ObjectsComparer.Comparer<T>();
            IEnumerable<Difference> differences;
            bool isEqual = comparer.Compare(t, s, out differences);
            result.IsEqual = isEqual;
            if (!isEqual)
            {
                string differencesMsg = string.Join(Environment.NewLine, differences);
                result.Msg = differencesMsg;
            }
            return result;
        }
        public class Result
        {
            public bool IsEqual { get; set; }
            public string Msg { get; set; }
        }

    }
}
