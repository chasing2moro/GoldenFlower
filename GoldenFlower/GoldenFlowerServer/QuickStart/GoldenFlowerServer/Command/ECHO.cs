using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class ECHO : CommandBase<CustomProtocolSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            // session.Send(Encoding.ASCII.GetString(requestInfo.Body) + Environment.NewLine);

            Console.WriteLine("收到前端消息:");
            defaultproto.example protoExample = UtilityProbuff.DeSerialize<defaultproto.example>(requestInfo.Body);
            Console.WriteLine(protoExample.name);
            Console.WriteLine(protoExample.field[0]);
            Console.WriteLine(protoExample.field[1]);
            Console.WriteLine(protoExample.gender);

            // Debug.Log(protoExample.year);
            // session.Send(requestInfo.Body, 0, requestInfo.Body.Length);

            defaultproto.account protoAccount = new defaultproto.account();
            protoAccount.name = "name acount";
            byte[] byteAcount = UtilityProbuff.Serialize(protoAccount);
            session.Send(byteAcount, 0, byteAcount.Length);
        }
    }
}
