using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class REGISTERACCOUNT : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            //对象池获取
            defaultproto.RepRegisterAcount repRegisterAcount_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepRegisterAcount>();

            defaultproto.ReqRegisterAcount req = UtilityProbuff.DeSerialize<defaultproto.ReqRegisterAcount>(requestInfo.Body);

            //user name existed?
            if (DatabaseManger.IsUserExist(req.username))
            {
                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.UserNameExist, "玩家名字已存在");
                //发送并回收
                SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, repRegisterAcount_pool);
                return;
            }

            // add to the user table
            List<DataBaseUser> userList = UtilityDataBase.Instance.ReadFullTable<DataBaseUser>(DataBaseUser.GetTableName());
            DataBaseUser user_pool = UtilityObjectPool.Instance.Dequeue<DataBaseUser>();  
            user_pool.id = userList.Count + 1;//玩家id，暂时这样写
            user_pool.username = req.username;
            user_pool.password = req.password;
            try
            {
                int result = UtilityDataBase.Instance.InsertValues<DataBaseUser>(DataBaseUser.GetTableName(), user_pool);
                Logger.Log("注册有结果：" + result);
                repRegisterAcount_pool.playerId = user_pool.id;
                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.None);
            }
            catch (Exception e)
            {
                Logger.Log("注册没结果 error:" + e.Message + " id:" + user_pool.id);
                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.InternalError, "can not insert user to table");
            }
     
            SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, repRegisterAcount_pool);

            //add to the resource table
            defaultproto.UpdateResource updateResource_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.UpdateResource>();
            //money
            defaultproto.Resource resourceMoney = new defaultproto.Resource();
            resourceMoney.type = defaultproto.ResourceType.Money;
            resourceMoney.count = 2000;
            //coin
            defaultproto.Resource resourceCoin = new defaultproto.Resource();
            resourceCoin.type = defaultproto.ResourceType.Coin;
            resourceCoin.count = 20;

            try
            {
                //db
                DataBaseReource resource_pool = UtilityObjectPool.Instance.Dequeue<DataBaseReource>();
                resource_pool.userid = user_pool.id;
                resource_pool.moneny = resourceMoney.count;
                resource_pool.coin = resourceCoin.count;
                int result = UtilityDataBase.Instance.InsertValues<DataBaseReource>(DataBaseReource.GetTableName(), resource_pool);
                UtilityObjectPool.Instance.Enqueue<DataBaseReource>(resource_pool);
                //protocol
                updateResource_pool.resource.Add(resourceMoney);
                updateResource_pool.resource.Add(resourceCoin);
                //UtilityMsgHandle.AssignErrorDes(updateResource_pool, defaultproto.ErrorCode.None);
            }
            catch (Exception e)
            {
                Logger.LogError("insert to server error:" + e.Message);
               // UtilityMsgHandle.AssignErrorDes(updateResource_pool, defaultproto.ErrorCode.InternalError, "insert to server error:" + e.Message);
            }
            session.SendProto(CommandName.UPDATERESOURCE, updateResource_pool);
            updateResource_pool.resource.Clear();
            UtilityObjectPool.Instance.Enqueue<defaultproto.UpdateResource>(updateResource_pool);
            //缓存玩家
            PlayerDataManager.Instance.AddPlayer(session, user_pool.id);
            PlayerDataManager.Instance.SetMoneyRaw(user_pool.id, resourceMoney.count);
            PlayerDataManager.Instance.SetCoinRaw(user_pool.id, resourceCoin.count);
            UtilityObjectPool.Instance.Enqueue<DataBaseUser>(user_pool);


    
        }
      }
}
