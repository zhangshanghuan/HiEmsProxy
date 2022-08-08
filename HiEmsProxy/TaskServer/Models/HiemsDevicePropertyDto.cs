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
        /// 描述 :数据层级 
        /// 空值 : true  
        /// </summary>
        public int? Level { get; set; }
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
        public string StartAddr { get; set; }
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
        /// 描述 :数据序号 
        /// 空值 : true  
        /// </summary>
        public int? DataIdx { get; set; }
        /// <summary>
        /// 描述 :数据个数 
        /// 空值 : true  
        /// </summary>
        public int? DataNum { get; set; }
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
    }

}
