using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HiEMS.Model.Dto;
using HiEMS.Model.Models;

namespace HiEMS.Model.Dto
{
    /// <summary>
    /// 设备属性输入对象
    /// @author jepen
    /// </summary>
    public class HiemsDevicePropertyDto
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 描述 :设备ID 
        /// 空值 : true  
        /// </summary>
        public int? DeviceId { get; set; }
        /// <summary>
        /// 描述 :属性名称 
        /// 空值 : true  
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述 :状态 
        /// 空值 : true  
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 描述 :描述 
        /// 空值 : true  
        /// </summary>
        public string Describe { get; set; }
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
        /// <summary>
        /// 描述 :刷新率 
        /// 空值 : true  
        /// </summary>
        public int? Refresh { get; set; }
        /// <summary>
        /// 描述 :从站ID 
        /// 空值 : true  
        /// </summary>
        public int? SlaveId { get; set; }
        /// <summary>
        /// 描述 :起始地址 
        /// 空值 : true  
        /// </summary>
        public int? StartAddr { get; set; }
        public int StartAddress { get; set; }
        /// <summary>
        /// 描述 :功能码 
        /// 空值 : true  
        /// </summary>
        public string Function { get; set; }
        /// <summary>
        /// 描述 :长度 
        /// 空值 : true  
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 描述 :数据格式 
        /// 空值 : true  
        /// </summary>
        public string DataFormat { get; set; }
        /// <summary>
        /// 描述 :数据枚举 
        /// 空值 : true  
        /// </summary>
        public string DataEmu { get; set; }
        /// <summary>
        /// 描述 :计算公式 
        /// 空值 : true  
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 描述 :单位 
        /// 空值 : true  
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 描述 :默认值 
        /// 空值 : true  
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 描述 :执行函数 
        /// 空值 : true  
        /// </summary>
        public string RunFunc { get; set; }
        /// <summary>
        /// 描述 :数据值总数 
        /// 空值 : true  
        /// </summary>
        public int? DataCount { get; set; }
        /// <summary>
        /// 描述 :读写模式 
        /// 空值 : true  
        /// </summary>
        public string RwType { get; set; }
        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }

        public string defaultVal { get; set; }
        public string lowerVal { get; set; }
        public string upperVal { get; set; }
    }

    /// <summary>
    /// 设备属性查询对象
    /// </summary>
    public class HiemsDevicePropertyQueryDto
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 设备类型ID 
        /// </summary>
        public int? DeviceId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 字典
        /// </summary>
        public string Dict { get; set; }
        /// <summary>
        /// 编码 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 数据等级
        /// </summary>
        public int? Rank { get; set; }
        /// <summary>
        /// 层级1序号
        /// </summary>
        public int? Gl1 { get; set; }
        /// <summary>
        /// 层级2序号
        /// </summary>
        public int? Gl2 { get; set; }
        /// <summary>
        /// 层级3序号
        /// </summary>
        public int? Gl3 { get; set; }
        /// <summary>
        /// 层级4序号
        /// </summary>
        public int? Gl4 { get; set; }
    }
}
