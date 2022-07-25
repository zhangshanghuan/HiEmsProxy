using System;
using System.Collections.Generic;
using SqlSugar;

namespace HiEmsProxy.TaskServer.Models
{
    /// <summary>
    /// ，数据实体对象
    /// @author admin
    /// @date 2022-07-18
    /// </summary>
    [SugarTable("hiems_device_property")]
    public class HiemsDeviceProperty
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 协议 ModbusTcp ModbusRtu 
        /// </summary>
        public string Procotol { get; set; }

        /// <summary>
        /// 描述 :属性名称 
        /// 空值 : true  
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述 :描述 
        /// 空值 : true  
        /// </summary>
        public string Describe { get; set; }

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
        /// 描述 :编码 
        /// 空值 : true  
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 描述 :层级 
        /// 空值 : true  
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 描述 :刷新率 
        /// 空值 : true  
        /// </summary>
        public int RefreshRate { get; set; }

        /// <summary>
        /// 描述 :从站ID 
        /// 空值 : true  
        /// </summary>
        public int SlaveId { get; set; }

        /// <summary>
        /// 描述 :起始地址 
        /// 空值 : true  
        /// </summary>
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
        public int Length { get; set; }

        /// <summary>
        /// 描述 :数据格式 Uint16 
        /// 空值 : true  
        /// </summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// 描述 :数据枚举 key1:value1,key2:value2... 
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
        /// 描述 :执行类型 
        /// 空值 : true  
        /// </summary>
        public string RunFunc { get; set; }

        /// <summary>
        /// 描述 :数据个数 N9 N1可以为空 
        /// 空值 : true  
        /// </summary>
        public int DataNum { get; set; }

        /// <summary>
        /// 描述 :数据值总数 C200 C数据用逗号隔开  C1可以为空 
        /// 空值 : true  
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 描述 :读写模式 
        /// 空值 : true  
        /// </summary>
        public string RwType { get; set; }

        /// <summary>
        /// 描述 :协议ID 
        /// 空值 : true  
        /// </summary>
        public int ProtocolId { get; set; }


        public string Value { get; set; }


        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }
    }
}