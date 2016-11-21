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

            //玩家名字是否重名
            if (DatabaseManger.IsUserExist(req.username))
            {
                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.UserNameExist, "玩家名字已存在");
                //发送并回收
                SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, repRegisterAcount_pool);
                return;
            }

            //玩家总量
            List<DataBaseUser> userList = UtilityDataBase.Instance.ReadFullTable<DataBaseUser>(DataBaseUser.GetTableName());

            DataBaseUser user_pool = UtilityObjectPool.Instance.Dequeue<DataBaseUser>();  //对象池拿出来
            user_pool.id = userList.Count + 1;//玩家id，暂时这样写
            user_pool.username = req.username;
            user_pool.password = req.password;
            try
            {
                int result = UtilityDataBase.Instance.InsertValues<DataBaseUser>(DataBaseUser.GetTableName(), user_pool);
                Logger.Log("注册有结果：" + result);

                //玩家id
                repRegisterAcount_pool.playerId = user_pool.id;

                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.None);
            }
            catch (Exception e)
            {
                Logger.Log("注册没结果 error:" + e.Message + " id:" + user_pool.id);
                UtilityMsgHandle.AssignErrorDes(repRegisterAcount_pool, defaultproto.ErrorCode.InternalError, "服务器内部错误");
                //throw;
            }

            //对象池回收
           UtilityObjectPool.Instance.Enqueue<DataBaseUser>(user_pool);

            //发送并回收
            SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, repRegisterAcount_pool);
        }
      }
}
