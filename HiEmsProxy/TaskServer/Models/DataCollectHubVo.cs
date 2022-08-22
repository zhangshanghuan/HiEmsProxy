using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using HiEMS.Model.Models;
using HiEMS.Model.Dto;

namespace HiEMS.Model.Business.Vo
{
    /// <summary>
    /// 数据采集实体类
    /// </summary>
    public class DataCollectHubVo
    {
        /// <summary>
        /// InfoId
        /// </summary>
        public int InfoId { get; set; }

        /// <summary>
        /// DeviceId
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// PropertyId
        /// </summary>
        public int LocalId { get; set; }

        /// <summary>
        /// PropertyId
        /// </summary>
        public int? ProtocolId { get; set; }

        /// <summary>
        /// Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public HiemsDeviceInfo Info { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public HiemsDeviceType Type { get; set; }

        /// <summary>
        /// 设备协议
        /// </summary>
        public HiemsDeviceProtocol Protocol { get; set; }

        /// <summary>
        /// 设备属性
        /// </summary>
        public List<HiemsDevicePropertyDto> Property { get; set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        public List<HiemsDeviceDataDtoForHub> Data { get; set; }
     
    }
}
