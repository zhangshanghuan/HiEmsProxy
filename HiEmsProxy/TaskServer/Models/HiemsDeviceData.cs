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
    [SugarTable("hiems_device_data")]
    public class HiemsDeviceData
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 描述 :路由 
        /// 空值 : true  
        /// </summary>
        public string Router { get; set; }

        /// <summary>
        /// 描述 :值 
        /// 空值 : true  
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 描述 :结果 
        /// 空值 : true  
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 描述 :R/W 
        /// 空值 : true  
        /// </summary>
        public string RwType { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }

    }
}