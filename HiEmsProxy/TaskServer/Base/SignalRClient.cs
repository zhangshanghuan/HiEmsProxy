using HiEmsProxy.TaskServer.Model;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace HiEmsProxy.TaskServer.Base
{ 
    //实现IRetryPolicy接口
    class RetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// 重连规则：重连次数<50：间隔1s;重试次数<250:间隔30s;重试次数>250:间隔1m
        /// </summary>
        /// <param name="retryContext"></param>
        /// <returns></returns>
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {      
            var count = retryContext.PreviousRetryCount / 50;
            Console.WriteLine("ReConnect  count"+count);
            if (count < 1)//重试次数<50,间隔1s
            {
                return new TimeSpan(0, 0, 1);
            }
            else if (count < 5)//重试次数<250:间隔10s
            {
                return new TimeSpan(0, 0, 10);
            }
            else //重试次数>250:间隔30s
            {
                return new TimeSpan(0, 0, 30);
            }
        }
    }

    public class SignalClien
    {
        AutoResetEvent _AutoResetEvent = new AutoResetEvent(false);
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        BaseConfig _BaseConfig = null;
        IHttpContextAccessor _httpContextAccessor=new HttpContextAccessor();
        public HubConnectionBuilder _connection;
        private HubConnection _HubConnection;
        public static string URL = "http://172.18.8.178:8888/dataHub";
        private SignalClien()
        {
            DelegateLib.SignalDevConDelegate += DevConnectDelegate;
            _BaseConfig = config.GetRequiredSection("BaseConfig").Get<BaseConfig>();
        }
        //单例模式
        private static SignalClien instance = null;
        public static SignalClien getInstance()
        {
            if (instance == null)
            {
                instance = new SignalClien();
            }
            return instance;
        }
     
        public async Task<bool>   Connect()
        {
            URL = _BaseConfig!=null? _BaseConfig.SignalRUrl: URL;
             _connection = new HubConnectionBuilder();           
            _connection.WithUrl(URL);
            //自定义重连规则实现           
            _connection.WithAutomaticReconnect(new RetryPolicy());
             _HubConnection =  _connection.Build();           
            _HubConnection.On<string>("GetMessage", (msg) => GetMessage(msg));
            _HubConnection.On<string>("GetDeviceProp", (msg) => GetDeviceProp(msg));
            _HubConnection.On<string>("GetRemote", (msg) => GetRemote(msg));
            _HubConnection.On<string>("GetAdmin", (msg) => GetAdmin(msg));
            try
            {               
                _HubConnection.Closed += _HubConnection_Closed;
                //_HubConnection.Reconnected += _HubConnection_Reconnected;            
                await _HubConnection.StartAsync();
                if (_HubConnection.State==HubConnectionState.Connected)
                {                                
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);              
            }
            return false;
        }
         //初始化信息交互
        public async Task<bool> InitMain()
        {             
           return await SetInit();        
        }

        //断开连接
        public async void Disconnect()
        {          
            try
            {         
                await _HubConnection.StopAsync();
                await _HubConnection.DisposeAsync();                          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

        }

        //连接状态
        public bool ConnectState()
        {
            if (_HubConnection!=null)
            {
                if (_HubConnection.State== HubConnectionState.Connected)
                {
                    return true;
                }
            }
            return false;
        }

        #region 接收
        public void GetMessage(string  msg)
        {
            Console.WriteLine("From {GetMessage} server:" + msg);       
        }
        public void GetDeviceProp(string msg)
        {
            Console.WriteLine("From  {GetDeviceProp}  server:" + msg);         
            if (DelegateLib.SignalDevicePropDelegate != null)
            {
                DelegateLib.SignalDevicePropDelegate(msg);
            }
        }
        public void GetRemote(string msg)
        {
            Console.WriteLine("From {GetRemote} server:" + msg);
            if (DelegateLib.SignalRemoteDelegate != null)
            {
                DelegateLib.SignalRemoteDelegate(msg);
            }
        }
        public void GetAdmin(string msg)
        {
            Console.WriteLine("From {GetAdmin} server:" + msg);
        }
        #endregion

        #region 发送
        public async Task<bool> SetInit()
        {
            try
            {
                DataCollectUser _DataCollectUser = new DataCollectUser()
                {
                    ConnectionId = _HubConnection.ConnectionId,
                    Name = _BaseConfig.Name,
                    LoginTime = DateTime.Now,
                    LocalIP = _BaseConfig.LocalIp,
                    LocalId = Convert.ToInt16(_BaseConfig.LocalId),   //采集执行ID
                    Location = _BaseConfig.Location,
                    UserType = _BaseConfig.UserType,
                };
                string data = JsonConvert.SerializeObject(_DataCollectUser);
                Console.WriteLine("SignalClient Connect Success！！！");
                await SendMsgToServer("SetInit", _HubConnection.ConnectionId, data);
             
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public  bool SetDeviceData(string data)
        {
            if (ConnectState())
            {
                return  SendMsgToServer("SetDeviceData", _HubConnection.ConnectionId, data).Wait(5000);
            }
            return false;
        }
        private async  Task SendMsgToServer(string MethodName,string ConnectionId,string data)
        {
            Console.WriteLine("Send server:" + data);
            await _HubConnection.InvokeAsync(MethodName, _HubConnection.ConnectionId, data);
        }
        #endregion

        #region 设备连接状态
        private void DevConnectDelegate(string StateStr)
        {
            Console.WriteLine(StateStr);
        }
        #endregion

        //断开连接事件
        private System.Threading.Tasks.Task _HubConnection_Closed(Exception arg)
        {
            Console.WriteLine("SignalClient DisConnect !!!");
            return null;
        }
        //重新连接
        private async Task _HubConnection_Reconnected(string arg)
        {
            await SetInit();
        }
    }
}
