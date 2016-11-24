using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using ProtoBuf;

public class CustomProtocolSession : AppSession<CustomProtocolSession, BinaryRequestInfo>
    {
        protected override void HandleException(Exception e)
        {

        }

    protected override void OnSessionClosed(CloseReason reason)
    {
        //del player
        PlayerDataManager.Instance.RemovePlayer(this);

        base.OnSessionClosed(reason);
    }


    byte[] GetHeaderByte(CommandName vCommandName)
    {
        string _header = UtilityMsg.GetHeaderByCommandName(vCommandName);
        byte[] _headerByte = Encoding.UTF8.GetBytes(_header);
        return _headerByte;
    }

    public void SendProto(CommandName vCommandName, IExtensible vProto)
    {           
        //包头
        byte[] byteHeader = GetHeaderByte(vCommandName);

        SendProtoWithByteHeader(byteHeader, vProto);
    }

    public void SendProtoWithByteHeader(byte[] vByteHeader, IExtensible vProto)
    {
        //包头
        byte[] byteHeader = vByteHeader;

        //总包
        byte[] byteSend;

        if (vProto != null)
        {
            //包体（从对象池取出来的)
            byte[] byteBody_pool = UtilityProbuff.Serialize(vProto);

            //总包
            byteSend = new byte[byteBody_pool.Length + byteHeader.Length];
            //coy包头
            Array.Copy(byteHeader, 0, byteSend, 0, 4);
            //copy包体
            Array.Copy(byteBody_pool, 0, byteSend, 4, byteBody_pool.Length);

            //放回缓存
            UtilityObjectPool.Instance.EnqueueBytes(byteBody_pool);
        }
        else
        {
            byteSend = byteHeader;
        }

        Send(byteSend, 0, byteSend.Length);
    }
}

