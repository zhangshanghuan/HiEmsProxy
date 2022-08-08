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
        /// 描述 :数据序号 
        /// 空值 : true  
        /// </summary>
        public int? DataIdx { get; set; }
        /// <summary>
        /// 描述 :属性名称 
        /// 空值 : true  
        /// </summary>
        public string Name { get; set; }
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
        /// 描述 :读写模式 
        /// 空值 : true  
        /// </summary>
        public string RwType { get; set; }
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

        public string Describe { get; set; }
        public string Code { get; set; }
    }

   
}
