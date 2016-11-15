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
                Type type = this.GetType();
                CommandName commandName = (CommandName)Enum.Parse(typeof(CommandName), type.Name);
                return UtilityMsg.GetHeaderByCommandName(commandName);
            }
        }

        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }
    }
}
