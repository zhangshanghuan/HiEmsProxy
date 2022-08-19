using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HiEMS.Model.Dto;
using HiEMS.Model.Models;

namespace HiEMS.Model.Dto
{
    /// <summary>
    /// 设备数据输入对象
    /// @author jepen
    /// </summary>
    public class HiemsDeviceDataDto
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public long Id { get; set; }
        /// <summary>
        /// 描述 :设备信息ID 
        /// 空值 : true  
        /// </summary>
        public int? InfoId { get; set; }
        /// <summary>
        /// 描述 :设备属性ID 
        /// 空值 : true  
        /// </summary>
        public int? PropertyId { get; set; }
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
        /// 描述 :数据来源 
        /// 空值 : true  
        /// </summary>
        public int? LocalId { get; set; }
        /// <summary>
        /// 描述 :插入时间 
        /// 空值 : true  
        /// </summary>
        public DateTime? AddTime { get; set; }
        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }
    }

    /// <summary>
    /// 设备数据输入对象
    /// @author jepen
    /// </summary>
    public class HiemsDeviceDataDtoForHub : HiemsDeviceDataDto
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 描述 :属性名称 
        /// 空值 : true  
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述 :读写模式 
        /// 空值 : true  
        /// </summary>
        public string RwType { get; set; }
        /// <summary>
        /// 描述 :字典 
        /// 空值 : true  
        /// </summary>
        public string Dict { get; set; }
        /// <summary>
        /// 描述 :编码 
        /// 空值 : true  
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 描述 :数据类型 
        /// 空值 : true  
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 描述 :数据等级 
        /// 空值 : true  
        /// </summary>
        public int? Rank { get; set; }
        /// <summary>
        /// 描述 :层级1序号 
        /// 空值 : true  
        /// </summary>
        public int? Gl1 { get; set; }
        /// <summary>
        /// 描述 :层级2序号 
        /// 空值 : true  
        /// </summary>
        public int? Gl2 { get; set; }
        /// <summary>
        /// 描述 :层级3序号 
        /// 空值 : true  
        /// </summary>
        public int? Gl3 { get; set; }
        /// <summary>
        /// 描述 :层级4序号 
        /// 空值 : true  
        /// </summary>
        public int? Gl4 { get; set; }
    }

    /// <summary>
    /// 设备数据查询对象
    /// </summary>
    public class HiemsDeviceDataQueryDto 
    {
        /// <summary>
        /// ID 
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 设备信息ID 
        /// </summary>
        public int? InfoId { get; set; }
        /// <summary>
        /// 属性ID 
        /// </summary>
        public int? PropertyId { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 采集器ID
        /// </summary>
        public int? LocalId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? AddTime { get; set; }
    }
}
