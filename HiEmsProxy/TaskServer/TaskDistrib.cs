using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiEmsProxy.TaskServer.Actuator;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer
{
    public class TaskDistrib
    {
        //任务执行器
        public List<Tasklib> _ListTaskLib = new List<Tasklib>();
        public bool IsStart = true;
        //采集队列
        public PriorityQueue<Tasklib, int> _PriorityQueue = new PriorityQueue<Tasklib, int>();
        //执行队列
        public PriorityQueue<Tasklib, int> _ExcutePriorityQueue = new PriorityQueue<Tasklib, int>(); 
        private HiemsDeviceProtocol _Config;   
    
        //将任务按照通讯方式进行分发到指定的执行器             
        public void DisToExecute(List<Tasklib> _ListTaskLib)
        {
            DelegateLib.manual.Reset();
            this._ListTaskLib = _ListTaskLib;
            QueueExecute("GetAll");
            for (int i = 0; i < _ListTaskLib.Count; i++)
            {
                Tasklib _Task  = _PriorityQueue.Dequeue();
                SelectExecute(_Task, "采集");
            }           
            DelegateLib.manual.Set();
        }
        //插入任务
        public void Insert(Tasklib _Task)
        {
            SelectExecute(_Task, "执行");
        }
        //更新所有任务
        public void UpdateAllTask(List<Tasklib> _ListTaskLib)
        {       
            foreach (var item in Commlib._ListRtuModbusLib)
            {
                item._TaskList.Clear();
            }
            foreach (var item in Commlib._ListTcpModbusLib)
            {
                item._TaskList.Clear();
            }
            foreach (var item in Commlib._ListSerialLib)
            {
                item._TaskList.Clear();
            }
            DisToExecute(_ListTaskLib);
        }
        //选择执行器
        private void SelectExecute(Tasklib _Task, string excutemain = "采集")
        {
            if (_Task == null) return;
            _Task.BronTime = DateTime.Now;
            _Config = _Task.DeviceProtocol;
            ActInterface _ActInterface = null;
            switch (_Task.DeviceProtocol.Protocol)
            {              
                case "ModbusRtu": //执行器
                    _ActInterface = Commlib.CreatModbusRtu(_Config.Portname, _Config.Baudrate, _Config.Databits, _Config.Parity, _Config.Stopbits);              
                    break;
                case "ModbusTcp"://执行器
                    _ActInterface = Commlib.CreatModbusTcp(_Config.Ip, _Config.Port);
                    break;
                case "Serial"://执行器
                    _ActInterface = Commlib.CreatSerial(_Config.Portname, _Config.Baudrate, _Config.Databits, _Config.Parity, _Config.Stopbits);                  
                    break;              
            }
            if (_ActInterface == null)
            {
                Console.WriteLine($"{_Task.Router} {_Config.Portname}  {_Config.Ip} {_Config.Port} 连接失败 ");
                return;
            }
            //处理DataCount 针对单体电芯连续读取，C400 连续读取400个。
            if (_Task.DeviceProperty.DataCount!=0)
            {
                _Task.DeviceProperty.Length = _Task.DeviceProperty.DataCount * _Task.DeviceProperty.Length;
            }
            if (excutemain == "采集") _ActInterface._TaskList.Add(_Task);
            if (excutemain == "执行") _ActInterface._ExcuteBlockingCollection.Add(_Task);
        }                         
        //更新队列
        object _lock = new object();
        private Tasklib QueueExecute(string type, Tasklib _TaskObject = null)
        {
            try
            {
                lock (_lock)
                {
                    switch (type)
                    {
                        case "Update"://更新任务              
                            var res = _ListTaskLib.Exists(t => t.Router == _TaskObject.Router);
                            if (res)
                            {
                                var model = _ListTaskLib.First(t => t.Router == _TaskObject.Router);
                                _ListTaskLib.Remove(model);
                                _ListTaskLib.Add(_TaskObject);
                            }
                            else
                            {
                                _ListTaskLib.Add(_TaskObject);
                            }
                            _PriorityQueue.Clear();
                            break;
                        case "GetSingle"://队列中获取一个优先级最高任务
                            if (_PriorityQueue.Count > 0) _TaskObject = _PriorityQueue.Dequeue();
                            break;
                        case "GetAll"://队列中插入所有任务
                            foreach (var item in _ListTaskLib)
                            {
                                _PriorityQueue.Enqueue(item, Convert.ToInt16(item.DeviceProperty.Level));
                            }
                            break;
                        case "Delete"://删除所有任务
                            _PriorityQueue.Clear();
                            break;
                        case "DeleteSingle"://删除单个任务
                            if (_ListTaskLib.Contains(_TaskObject)) _ListTaskLib.Remove(_TaskObject);
                            break;
                        case "Insert"://插入任务
                            _ExcutePriorityQueue.Enqueue(_TaskObject, Convert.ToInt16(_TaskObject.DeviceProperty.Level));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("分发器报错：" + ex.ToString());
            }
            return _TaskObject;
        }       
    }
}
