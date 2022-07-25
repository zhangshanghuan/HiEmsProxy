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
        /// 描述 :属性ID 
        /// 空值 : true  
        /// </summary>
        public int PropertyId { get; set; }

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
        /// 描述 :项目ID  deptId 
        /// 空值 : true  
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 描述 :采集器ID 
        /// 空值 : true  
        /// </summary>
        public string LocalId { get; set; }

        /// <summary>
        /// 描述 :父ID 
        /// 空值 : true  
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 描述 :路由路径 
        /// 空值 : true  
        /// </summary>
        public string Router { get; set; }

        /// <summary>
        /// 描述 :设备层级 
        /// 空值 : true  
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }
    }
}