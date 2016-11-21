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
            defaultproto.RepRegisterAcount rep = UtilityObjectPool.Instance.Dequeue<defaultproto.RepRegisterAcount>();

            defaultproto.ReqRegisterAcount req = UtilityProbuff.DeSerialize<defaultproto.ReqRegisterAcount>(requestInfo.Body);

            //玩家名字是否重名
            List<DataBaseUser> dataBaseUser = UtilityDataBase.Instance.ReadTable<DataBaseUser>(DataBaseUser.GetTableName(),
                new string[] { "username" },
                new string[] { "=" },
                new string[] { req.username });
            if (!dataBaseUser.IsNullOrEmpty())
            {
                UtilityMsgHandle.AssignErrorDes(rep, defaultproto.ErrorCode.UserNameExist, "玩家名字已存在");
                //发送并回收
                SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, rep);
                return;
            }


            List<DataBaseUser> userList = UtilityDataBase.Instance.ReadFullTable<DataBaseUser>(DataBaseUser.GetTableName());
            DataBaseUser user = new DataBaseUser();
            user.id = userList.Count + 1;
            user.username = req.username;
            user.password = req.password;
  
            try
            {
                int result = UtilityDataBase.Instance.InsertValues<DataBaseUser>(DataBaseUser.GetTableName(), user);
                Logger.Log("注册有结果：" + result);
  
                UtilityMsgHandle.AssignErrorDes(rep, defaultproto.ErrorCode.None);
            }
            catch (Exception e)
            {
                Logger.Log("注册没结果 error:" + e.Message + " id:" + user.id);
                UtilityMsgHandle.AssignErrorDes(rep, defaultproto.ErrorCode.InternalError, "服务器内部错误");
                //throw;
            }

            //发送并回收
            SessionSendWithRecycle<defaultproto.RepRegisterAcount>(session, rep);
        }
      }
}
