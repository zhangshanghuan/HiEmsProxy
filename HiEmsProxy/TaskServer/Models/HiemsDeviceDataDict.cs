using System;
using System.Collections.Generic;
using SqlSugar;

namespace HiEmsProxy.TaskServer.Models
{
    /// <summary>
    /// ，数据实体对象
    ///
    /// @author admin
    /// @date 2022-07-18
    /// </summary>
    [SugarTable("hiems_device_data_dict")]
    public class HiemsDeviceDataDict
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 描述 :属性ID 
        /// 空值 : true  
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// 描述 :路由路径 
        /// 空值 : true  
        /// </summary>
        public string Router { get; set; }

        /// <summary>
        /// 描述 :类型枚举 
        /// 空值 : true  
        /// </summary>
        public string TypeEmu { get; set; }

        /// <summary>
        /// 描述 :类型名称 
        /// 空值 : true  
        /// </summary>
        public string Typename { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }
    }
}