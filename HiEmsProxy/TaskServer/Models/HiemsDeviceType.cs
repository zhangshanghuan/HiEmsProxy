using System;
using System.Collections.Generic;
using SqlSugar;


namespace HiEMS.Model.Models
{
    /// <summary>
    /// 设备类型数据实体对象
    /// @author jepen
    /// </summary>
    [SugarTable("hiems_device_type")]
    public class HiemsDeviceType
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 描述 :名称 
        /// 空值 : true  
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述 :描述 
        /// 空值 : true  
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 描述 :状态 
        /// 空值 : true  
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 描述 :编码 
        /// 空值 : true  
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 描述 :协议类型 
        /// 空值 : true  
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// 描述 :设备类型 
        /// 空值 : true  
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }



    }
}