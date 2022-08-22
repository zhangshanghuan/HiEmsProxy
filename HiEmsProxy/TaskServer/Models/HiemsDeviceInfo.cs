using System;
using System.Collections.Generic;
using SqlSugar;


namespace HiEMS.Model.Models
{
    /// <summary>
    /// 设备信息数据实体对象
    /// @author jepen
    /// </summary>
    [SugarTable("hiems_device_info")]
    public class HiemsDeviceInfo
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 描述 :设备ID 
        /// 空值 : true  
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// 描述 :状态 
        /// 空值 : true  
        /// </summary>
        public string Status { get; set; }

        public string State { get; set; }
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
        /// 描述 :项目ID 
        /// 空值 : true  
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// 描述 :采集器ID 
        /// 空值 : true  
        /// </summary>
        public int? LocalId { get; set; }

        /// <summary>
        /// 描述 :父ID 
        /// 空值 : true  
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 描述 :路由路径 
        /// 空值 : true  
        /// </summary>
        public string Router { get; set; }

        /// <summary>
        /// 描述 :设备层级 
        /// 空值 : true  
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 描述 :协议ID 
        /// 空值 : true  
        /// </summary>
        public int? ProtocolId { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }



    }
}