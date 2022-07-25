
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

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
        public SignalClien()
        {
            Connect();
        }
        public HubConnectionBuilder _connection;
        private HubConnection _HubConnection;
        public static string URL="http://172.18.8.178:8889/dataHub";         
 
        public async Task<bool>   Connect()
        {
             _connection = new HubConnectionBuilder();
            _connection.WithUrl(URL);
            //自定义重连规则实现           
            _connection.WithAutomaticReconnect(new RetryPolicy());
            _HubConnection =  _connection.Build();
            _HubConnection.On<string>("GetMessage", (msg) => GetMessage(msg));
            _HubConnection.On<string>("Welcome", (msg) => WelcomeRecive(msg));
            try
            {
                _HubConnection.Closed += _HubConnection_Closed;               
                await _HubConnection.StartAsync();
                if (_HubConnection.State==HubConnectionState.Connected)
                {
                    Console.WriteLine("SignalClient Connect Success！！！");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);              
            }
            return false;
        }

        //重新连接
        private System.Threading.Tasks.Task _HubConnection_Closed(Exception arg)
        {
            Console.WriteLine("SignalClient DisConnect !!!");
            return null;
        }
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
        public  void GetMessage(string  msg)
        {
            Console.WriteLine("GetMessage HUB:" + msg);
            
        }
        public void WelcomeRecive(string msg)
        {
            Console.WriteLine("Welcome HUB:" + msg);
            _HubConnection.InvokeAsync("SendMessage", _HubConnection.ConnectionId, "123");
        }
    }
}
