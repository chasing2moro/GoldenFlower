using SuperSocket.SocketBase.Protocol;
using System;


namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class REGISTER_ACCOUNT : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            // session.Send(Encoding.ASCII.GetString(requestInfo.Body) + Environment.NewLine);

          
            defaultproto.register_acount registerAcount = UtilityProbuff.DeSerialize<defaultproto.register_acount>(requestInfo.Body);
            Console.WriteLine(registerAcount.username);
            Console.WriteLine(registerAcount.password);

            DataBaseUser user = new DataBaseUser();
            user.id = DateTime.Now.Millisecond;
            user.username = registerAcount.username;
            user.password = registerAcount.password;
           int result =  UtilityDataBase.Instance.InsertValues<DataBaseUser>(DataBaseUser.GetTableName(), user);
            Logger.Log("注册结果：" + result);
        }
      }
}
