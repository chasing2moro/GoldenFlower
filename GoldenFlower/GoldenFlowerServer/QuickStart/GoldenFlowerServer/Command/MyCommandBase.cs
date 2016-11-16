using ProtoBuf;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
   public class MyCommandBase : CommandBase<CustomProtocolSession, BinaryRequestInfo>
    {
        public override string Name
        {
            //收到 4个字节的协议头，转成协议Command
            get {
                return GetHeader();
            }
        }

        //0001/0002
        string GetHeader()
        {
            Type type = this.GetType();
            CommandName commandName = (CommandName)Enum.Parse(typeof(CommandName), type.Name);
            return UtilityMsg.GetHeaderByCommandName(commandName);
        }

        byte[] GetHeaderByte()
        {
            return Encoding.UTF8.GetBytes(GetHeader());
        }

        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }

        public void SessionSend(CustomProtocolSession Session, IExtensible vProto)
        {
            byte[] byteHeader = GetHeaderByte();
            byte[] byteBody = UtilityProbuff.Serialize(vProto);
            byte[] byteSend = new byte[byteBody.Length + byteHeader.Length];

            Array.Copy(byteHeader, 0, byteSend, 0, 4);
            Array.Copy(byteBody, 0, byteSend, 4, byteBody.Length);
            Session.Send(byteSend, 0, byteSend.Length);
        }
    }
}
