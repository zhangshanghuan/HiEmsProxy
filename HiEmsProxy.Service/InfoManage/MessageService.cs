﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using SqlSugar;
using WaterCloud.Domain.InfoManage;
using Microsoft.AspNetCore.SignalR;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Service.SystemManage;
using System.Net.Http;
using WaterCloud.Domain.SystemManage;
using System.Text;
using WaterCloud.DataBase;

namespace WaterCloud.Service.InfoManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-29 16:41
    /// 描 述：通知管理服务类
    /// </summary>
    public class MessageService : DataFilterService<MessageEntity>, IDenpendency
    {
        private string cacheHubKey = GlobalContext.SystemConfig.ProjectPrefix + "_hubuserinfo_";

        public IHubContext<MessageHub> _messageHub { get; set; }
        public ItemsDataService itemsApp { get; set; }
        public IHttpClientFactory _httpClientFactory { get; set; }
        public MessageService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        #region 获取数据
        public async Task<List<MessageEntity>> GetList(string keyword = "")
        {
            var query = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_MessageInfo.Contains(keyword) || a.F_CreatorUserName.Contains(keyword));
            }
            return await query.Where(a => a.F_EnabledMark == true).OrderBy(a => a.F_CreatorTime,OrderByType.Desc).ToListAsync();
        }

        public async Task<List<MessageEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(a => a.F_EnabledMark == true);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_MessageInfo.Contains(keyword) || a.F_CreatorUserName.Contains(keyword));
            }
            query = GetDataPrivilege("a","", query);
            return await query.OrderBy(a => a.F_CreatorTime,OrderByType.Desc).ToListAsync();
        }

        public async Task<List<MessageEntity>> GetUnReadListJson()
        {
            var hisquery = repository.Db.Queryable<MessageHistoryEntity>().Where(a => a.F_CreatorUserId == currentuser.UserId).Select(a => a.F_MessageId).ToList();
            var tempList= repository.Db.Queryable<MessageEntity, MessageHistoryEntity>((a,b) => new JoinQueryInfos(
                JoinType.Inner,a.F_Id==b.F_MessageId&&a.F_MessageType==2
                )).Select(a => a.F_Id).ToList();
            hisquery.AddRange(tempList);
            var query = repository.IQueryable(a => (a.F_ToUserId.Contains(currentuser.UserId) || a.F_ToUserId == "") && a.F_EnabledMark == true).Where(a => !hisquery.Contains(a.F_Id));
            return await GetFieldsFilterDataNew("a", query.OrderBy(a => a.F_CreatorTime,OrderByType.Desc)).ToListAsync();
        }

        public async Task<List<MessageEntity>> GetLookList(SoulPage<MessageEntity> pagination, string keyword = "")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("MessageType");
            Dictionary<string, string> messageTypeTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                messageTypeTemp.Add(item.F_ItemCode,item.F_ItemName);
            }
            dic.Add("F_MessageType", messageTypeTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = repository.IQueryable().Where(a => a.F_EnabledMark == true);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(a => a.F_MessageInfo.Contains(keyword) || a.F_CreatorUserName.Contains(keyword));
            }
            query = GetDataPrivilege("a","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<MessageEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<MessageEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(MessageEntity entity)
        {
            entity.Create();
            entity.F_EnabledMark = true;
            entity.F_CreatorUserName = currentuser.UserName;
            MessageEntity messageEntity = new MessageEntity();
            if (string.IsNullOrEmpty(entity.F_ToUserId))
            {
                string msg = entity.ToJson();
                entity.F_ToUserName = "所有人";
                entity.F_ToUserId = "";
                messageEntity = await repository.Insert(entity);
            }
            else
            {
                var users = entity.F_ToUserId.Split(",");
                entity.F_ToUserName = string.Join(",", repository.Db.Queryable<UserEntity>().Where(a => users.Contains(a.F_Id)).Select(a => a.F_RealName).ToList());
                messageEntity= await repository.Insert(entity);
            }
            //通过http发送消息
            messageEntity.companyId = currentuser.CompanyId;
            var mouduleName = ReflectionHelper.GetModuleName();
            var module = repository.Db.Queryable<ModuleEntity>().First(a => a.F_EnCode == mouduleName);
            var url = module.F_UrlAddress.Substring(0, module.F_UrlAddress.Length - 5) + "SendWebSocketMsg";
            HttpContent httpContent = new StringContent(messageEntity.ToJson(), Encoding.UTF8);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var result = _httpClientFactory.CreateClient().PostAsync(GlobalContext.SystemConfig.MainProgram + url, httpContent).GetAwaiter().GetResult();

        }
        public async Task SendWebSocketMsg(MessageEntity messageEntity)
        {
            if (!string.IsNullOrEmpty(messageEntity.companyId) && messageEntity.F_ToUserId.Length == 0)
            {
                await _messageHub.Clients.Group(messageEntity.companyId).SendAsync("ReceiveMessage", messageEntity.ToJson());
            }
            else
            {
                var users = messageEntity.F_ToUserId.Split(',');
                foreach (var item in users)
                {
                    //存在就私信
                    var connectionIDs = await CacheHelper.GetAsync<List<string>>(cacheHubKey + item);
                    if (connectionIDs == null)
                    {
                        continue;
                    }
					foreach (var connectionID in connectionIDs)
					{
                        await _messageHub.Clients.Client(connectionID).SendAsync("ReceiveMessage", messageEntity.ToJson());
                    }
                }
            }
        }
        public async Task ReadAllMsgForm(int type)
        {
            var unList=await GetUnReadListJson();
            var strList = unList.Where(a => a.F_MessageType == type&&a.F_ClickRead==true).Select(a=>a.F_Id).ToList();
            unitofwork.CurrentBeginTrans();
            foreach (var item in strList)
            {
               await ReadMsgForm(item);
            }
            unitofwork.CurrentCommit();
        }

        public async Task ReadMsgForm(string keyValue)
        {            
            MessageHistoryEntity msghis = new MessageHistoryEntity();
            msghis.Create();
            msghis.F_CreatorUserName = currentuser.UserName;
            msghis.F_MessageId = keyValue;
            await repository.Db.Insertable(msghis).ExecuteCommandAsync();
        }

        public async Task<bool> CheckMsg(string keyValue)
        {
            var msg = await repository.FindEntity(keyValue);
            if (msg==null)
            {
                return true;
            }
            if (msg.F_ClickRead==false)
            {
                return true;
            }
            if (await repository.Db.Queryable<MessageHistoryEntity>().Where(a => a.F_MessageId == keyValue && a.F_CreatorUserId == currentuser.UserId).AnyAsync())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Update(a => ids.Contains(a.F_Id), a=>new MessageEntity { 
                F_EnabledMark=false         
            });
        }
        #endregion

    }
}
