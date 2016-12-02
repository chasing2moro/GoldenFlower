using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class LOGIN : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            //对象池获取
            defaultproto.RepLogin repLogin_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepLogin>();

            //玩家请求信息
            defaultproto.ReqLogin req = UtilityProbuff.DeSerialize<defaultproto.ReqLogin>(requestInfo.Body);

            //玩家名字是否有
            if (!DatabaseManger.IsUserExist(req.username))
            {
                UtilityMsgHandle.AssignErrorDes(repLogin_pool, defaultproto.ErrorCode.UserNameNotCorrect, "账号不正确");
                //发送并回收
                SessionSendWithRecycle<defaultproto.RepLogin>(session, repLogin_pool);
                return;
            }

            //玩家名字&密码是否正确
            int userId;
            if (!DatabaseManger.IsUserExist(req.username, req.password,out userId))
            {
                UtilityMsgHandle.AssignErrorDes(repLogin_pool, defaultproto.ErrorCode.PasswordNotCorrect, "密码不正确");
                //发送并回收
                SessionSendWithRecycle<defaultproto.RepLogin>(session, repLogin_pool);
                return;
            }

            repLogin_pool.playerId = userId;
            UtilityMsgHandle.AssignErrorDes(repLogin_pool, defaultproto.ErrorCode.None, "登陆成功");

            //缓存玩家
            PlayerDataManager.Instance.AddPlayer(session, userId);

            //read resource table
            DataBaseReource resource = DatabaseManger.GetUserResource(userId);
            PlayerDataManager.Instance.SetMoneyRaw(userId, resource.moneny);
            PlayerDataManager.Instance.SetCoinRaw(userId, resource.coin);
            defaultproto.UpdateResource updateResource_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.UpdateResource>();
            defaultproto.Resource resourceMoney = new defaultproto.Resource();
            resourceMoney.type = defaultproto.ResourceType.Money;
            resourceMoney.count = resource.moneny;
            defaultproto.Resource resourceCoin = new defaultproto.Resource();
            resourceCoin.type = defaultproto.ResourceType.Coin;
            resourceCoin.count = resource.coin;
            updateResource_pool.resource.Add(resourceMoney);
            updateResource_pool.resource.Add(resourceCoin);


            //发送并回收
            SessionSendWithRecycle<defaultproto.RepLogin>(session, repLogin_pool);

            session.SendProto(CommandName.UPDATERESOURCE, updateResource_pool);// send to user
            updateResource_pool.resource.Clear();
            UtilityObjectPool.Instance.Enqueue<defaultproto.UpdateResource>(updateResource_pool);
        }
    }
}
