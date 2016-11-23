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

            //发送并回收
            SessionSendWithRecycle<defaultproto.RepLogin>(session, repLogin_pool);
        }
    }
}
