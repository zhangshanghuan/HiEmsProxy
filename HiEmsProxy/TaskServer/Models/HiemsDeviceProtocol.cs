using System;
using System.Collections.Generic;
using System.IO.Ports;
using SqlSugar;


namespace HiEMS.Model.Models
{
    /// <summary>
    /// 设备协议数据实体对象
    /// @author jepen
    /// </summary>
    [SugarTable("hiems_device_protocol")]
    public class HiemsDeviceProtocol
    {
        /// <summary>
        /// 描述 :ID 
        /// 空值 : false  
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int? Id { get; set; }

        /// <summary>
        /// 描述 :状态 
        /// 空值 : true  
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 描述 :协议类型 
        /// 空值 : true  
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// 描述 :IP地址 
        /// 空值 : true  
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 描述 :IP端口 
        /// 空值 : true  
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// 描述 :串口名 
        /// 空值 : true  
        /// </summary>
        public string Portname { get; set; }

        /// <summary>
        /// 描述 :波特率 
        /// 空值 : true  
        /// </summary>
        public int? Baudrate { get; set; }

        /// <summary>
        /// 描述 :数据位 
        /// 空值 : true  
        /// </summary>
        public int? Databits { get; set; }

        /// <summary>
        /// 描述 :校验位 
        /// 空值 : true  
        /// </summary>
        public int? Parity { get; set; }

        /// <summary>
        /// 描述 :停止位 
        /// 空值 : true  
        /// </summary>
        public int? Stopbits { get; set; }

        /// <summary>
        /// 描述 :附加信息 
        /// 空值 : true  
        /// </summary>
        public string Attach { get; set; }



    }
}