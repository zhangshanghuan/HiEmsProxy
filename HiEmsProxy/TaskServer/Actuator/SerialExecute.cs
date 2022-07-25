﻿using HithiumEmsPlatForm.BLL.TaskServer;
using Serial_Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer.Actuator
{
    public class SerialExecute: ActInterface
    {
        public ConcurrentDictionary<string, DateTime> _list = new();
        public SerialLib _SerialLib=new SerialLib();
        readonly Modbushelp _modbushelp = new();
        //执行集合
        public BlockingCollection<Tasklib> _ExcuteBlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);
        //采集集合
        public List<Tasklib> _TaskList { get; set; } = new List<Tasklib>();
        public BlockingCollection<Tasklib> _BlockingCollection { get; set; } = new BlockingCollection<Tasklib>(1000);

        public SerialExecute(SerialPort _SerialPort)
        {
            bool res=  _SerialLib.Init(_SerialPort);
            if (res) Main();
        }       
        public void Main()       
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    for (int i = 0; i < _TaskList.Count; i++)
                    {
                        DelegateLib.manual.WaitOne();
                        Tasklib _Tasklib = null;
                        ResultLib _ResultLib = null;
                        if (_ExcuteBlockingCollection.Count > 0)
                        {
                            //优先执行
                            _Tasklib = _ExcuteBlockingCollection.Take();
                            _ResultLib = Set(_Tasklib.DeviceProperty);
                            _modbushelp.ExecuteEvent(_ResultLib);
                        }
                        else
                        {
                            if (_TaskList.Count > i) _Tasklib = _TaskList[i];    //_Tasklib = _BlockingCollection.Take();
                            // 判断是否在刷新间隔内
                            bool res = _modbushelp.CheckRefreshInterval(_list, _Tasklib.Router, _Tasklib.DeviceProperty.RefreshRate, DateTime.Now); if (!res) continue;
                            _ResultLib = Set(_Tasklib.DeviceProperty);
                            _list.TryAdd(_Tasklib.Router, DateTime.Now);
                        }
                        //数据上传
                        if (_ResultLib != null)
                        {
                            _Tasklib.BronTime = DateTime.Now;
                            _ResultLib.Router = _Tasklib.Router;
                            _modbushelp.UploadEvent(_ResultLib);
                        }
                    }
                }
            });
        }
    
        private ResultLib Set(HiemsDeviceProperty DeviceProperty)
        {
            ResultLib _ResultLib = new();
            //判断是否连接成功
            if (_SerialLib == null || !_SerialLib.IsConnected)
            {
                bool RES = _SerialLib.ReConnect();
                if (!RES)
                {                  
                    return _ResultLib;
                }
            }
            //执行对应的方法


            return _ResultLib;
        }

        //激活
        public void ActiveMain()
        {
            _SerialLib.SendDataMethod("Y");
        }
  
        //读取电池电压
        public string GetBatteryVoltage()
        {         
          return  _SerialLib.GetData("B");
        }

        //读取输入电压
        public string GetInputVoltage()
        {
           return _SerialLib.GetData("L");
        }
        //输出电压
        public string GetOutputVoltage()
        {
            return _SerialLib.GetData("O");
        }
        //负载水平
        public string GetLoadPower()
        {
            return _SerialLib.GetData("P");
        }

        //所连电池的总Ah容量
        public string GetBatteryCapacity()
        {
            return _SerialLib.GetData("f");
        }
        //剩余电池运行时间
        public string GetRunLefttime()
        {
            return _SerialLib.GetData("j");
        }
        //输入频率
        public string GetInputFrequency()
        {
            return _SerialLib.GetData("L");
        }
        //环境温度
        public string GetInerTemperature()
        {
            return _SerialLib.GetData("C");
        }
                                              
    }
}