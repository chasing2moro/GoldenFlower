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
            //收到 4个字节的包头，转成协议Command
            get
            {
                return GetHeader();
            }
        }

        //0001/0002
        string _header;
        /// <summary>
        /// 包头
        /// </summary>
        /// <returns></returns>
        string GetHeader()
        {
            if (string.IsNullOrEmpty(_header))
            {
                Type type = this.GetType();
                CommandName commandName = (CommandName)Enum.Parse(typeof(CommandName), type.Name);
                _header =  UtilityMsg.GetHeaderByCommandName(commandName);
            }
            return _header;
        }

        byte[] _headerByte;
        /// <summary>
        /// 包头byte[]
        /// </summary>
        /// <returns></returns>
        byte[] GetHeaderByte()
        {
            if (_headerByte.IsNullOrEmpty())
            {
                _headerByte = Encoding.UTF8.GetBytes(GetHeader());
            }
            return _headerByte;
        }

        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 封装过的发送接口，所有的发送都经过这里，要不然前端不识别
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="vProto"></param>
        public void SessionSend(CustomProtocolSession Session, IExtensible vProto)
        {
            //包头
            byte[] byteHeader = GetHeaderByte();
            //包体（从对象池取出来的)
            byte[] byteBody_pool = UtilityProbuff.Serialize(vProto);

            //总包
            byte[] byteSend = new byte[byteBody_pool.Length + byteHeader.Length];
            //coy包头
            Array.Copy(byteHeader, 0, byteSend, 0, 4);
            //copy包体
            Array.Copy(byteBody_pool, 0, byteSend, 4, byteBody_pool.Length);

           //放回缓存
           UtilityObjectPool.Instance.EnqueueBytes(byteBody_pool);

            Session.Send(byteSend, 0, byteSend.Length);
        }


        /// <summary>
        /// 封装过的发送接口，所有的发送都经过这里，要不然前端不识别
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="vProto"></param>
        public void SessionSendWithRecycle<T>(CustomProtocolSession Session, T vProto) where T: IExtensible
        {
            SessionSend(Session, vProto);
            //对象池回收
            UtilityObjectPool.Instance.Enqueue<T>((T)vProto);
        }
    }
}
