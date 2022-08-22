using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiEMS.Model.Models;
using HiEmsProxy.TaskServer.Actuator;
using HiEmsProxy.TaskServer.Base;
using HiEmsProxy.TaskServer.Model;
using HiEmsProxy.TaskServer.Models;

namespace HiEmsProxy.TaskServer
{
    /// <summary>
    /// 任务分发器
    /// </summary>
    public class TaskDistrib
    {
        //任务执行器
        public List<Tasklib> _tasklibs;
        //采集队列
        public PriorityQueue<Tasklib, int> _PriorityQueue = new PriorityQueue<Tasklib, int>();
        //执行队列
        public PriorityQueue<Tasklib, int> _ExcutePriorityQueue = new PriorityQueue<Tasklib, int>(); 
        private HiemsDeviceProtocol _DeviceProtocol;   
    
        //将任务按照通讯方式进行分发到指定的执行器             
        public void DisToExecute(List<Tasklib> _tasks)
        {
            DelegateLib.manual.Reset();
            _tasklibs = _tasks;
            //将任务插入优先级队列，进行排序
            QueueExecute("GetAll");
            //清除所有执行器中的任务队列
            ClearTaskSet();
            for (int i = 0; i < _tasklibs.Count; i++)
            {
                //从优先级队列取出
                Tasklib _Task  = _PriorityQueue.Dequeue();          
               //给执行器分发任务
                SelectExecute(_Task, "采集");
            }           
            DelegateLib.manual.Set();
        }  
        //插入任务
        public void Insert(Tasklib _Task)
        {
            SelectExecute(_Task, "执行");
        }    
        //选择执行器
        private void SelectExecute(Tasklib _Task, string excutemain = "采集")
        {
            if (_Task == null) return;
            _DeviceProtocol = _Task.DeviceProtocol;
            ActInterface _ActInterface = null;
            int ProtocolID = (int)_Task.DeviceProtocol.Id;
            switch (_Task.DeviceProtocol.Protocol.ToLower())//全部转为小写
            {              
                case "modbusrtu": //执行器
                    _ActInterface = Commlib.CreatModbusRtu(_DeviceProtocol.Portname, (int)_DeviceProtocol.Baudrate, (int)_DeviceProtocol.Databits, (Parity)_DeviceProtocol.Parity,(StopBits)_DeviceProtocol.Stopbits, ProtocolID);              
                    break;
                case "modbustcp"://执行器
                    _ActInterface = Commlib.CreatModbusTcp(_DeviceProtocol.Ip, (int)_DeviceProtocol.Port, ProtocolID);
                    break;
                case "serial"://执行器
                    _ActInterface = Commlib.CreatSerial(_DeviceProtocol.Portname, (int)_DeviceProtocol.Baudrate, (int)_DeviceProtocol.Databits, (Parity)_DeviceProtocol.Parity, (StopBits)_DeviceProtocol.Stopbits, ProtocolID);                  
                    break;     
                    default:
                    Console.WriteLine($"没有找到对应的执行器：{_Task.DeviceProtocol.Protocol} ");
                    return;
            }
            if (_ActInterface == null)
            {
                Console.WriteLine($"{_Task.Router} {_DeviceProtocol.Portname}  {_DeviceProtocol.Ip} {_DeviceProtocol.Port} 连接失败 ");
                return;
            }
            //处理DataCount 针对单体电芯连续读取，C400 连续读取400个。
            if (_Task.DeviceProperty.DataCount>1)
            {
                _Task.DeviceProperty.Length = _Task.DeviceProperty.DataCount * _Task.DeviceProperty.Length;
            }
            if (excutemain == "采集") _ActInterface._TaskList.Add(_Task);
            if (excutemain == "执行") _ActInterface._ExcuteBlockingCollection.Add(_Task);
        }                         

       //清除所有执行器中的任务队列
       public  void ClearTaskSet()
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
                            var res = _tasklibs.Exists(t => t.Router == _TaskObject.Router);
                            if (res)
                            {
                                var model = _tasklibs.First(t => t.Router == _TaskObject.Router);
                                _tasklibs.Remove(model);
                                _tasklibs.Add(_TaskObject);
                            }
                            else
                            {
                                _tasklibs.Add(_TaskObject);
                            }
                            _PriorityQueue.Clear();
                            break;
                        case "GetSingle"://队列中获取一个优先级最高任务
                            if (_PriorityQueue.Count > 0) _TaskObject = _PriorityQueue.Dequeue();
                            break;
                        case "GetAll"://队列中插入所有任务
                            foreach (var item in _tasklibs)
                            {
                                int level = item.DeviceProperty.Rank == null ? 1 : (int)item.DeviceProperty.Rank;                          
                                _PriorityQueue.Enqueue(item, level);
                            }
                            break;
                        case "Delete"://删除所有任务
                            _PriorityQueue.Clear();
                            break;
                        case "DeleteSingle"://删除单个任务
                            if (_tasklibs.Contains(_TaskObject)) _tasklibs.Remove(_TaskObject);
                            break;
                        case "Insert"://插入任务
                            _ExcutePriorityQueue.Enqueue(_TaskObject, _TaskObject.DeviceProperty.Rank==null?1:(int)_TaskObject.DeviceProperty.Rank);
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
