using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using ProtoBuf;

/// <summary>
/// socket 解析，主要负责 字节流解析出命令号 和 probuff
/// </summary>
public class SocketParser
{
    string __header;//0000、0001
    int __bodyLen;
    int __leftLen;//剩余字节
    CommandName __commandName;//命令号
    Type __type;//proto buff 类
    Dictionary<CommandName, Type> _commandName2Type = new Dictionary<CommandName, Type>()
   {
       {CommandName.ADD,                typeof(defaultproto.account) },
       {CommandName.ECHO,               typeof(defaultproto.account) },
       {CommandName.MUTL,               typeof(defaultproto.account) },
       {CommandName.REGISTERACCOUNT,    typeof(defaultproto.RepRegisterAcount) },
       {CommandName.LOGIN,              typeof(defaultproto.RepLogin) },
       {CommandName.BET,                typeof(defaultproto.RepBet) },
       {CommandName.JOININBATTLE,       typeof(defaultproto.RepJoinBattle) },
       {CommandName.UPDATEDEALCARD,     typeof(defaultproto.UpdateDealCard) },
       {CommandName.QUIT,               typeof(defaultproto.RepQuit) },
   };

    byte[] _leftBytes;//剩余没解析完的

    //包头和包体长度
    static int HeaderOffset = 4;
    //包头和包体长度
    static int HeaderAndBodyLenOffset = 6;

    public void ParserRawData(byte[] vRawData)
    {

        if (!_leftBytes.IsNullOrEmpty())
        {
            if (!vRawData.IsNullOrEmpty())
            {
                //粘包
                byte[] newLeftBytes = new byte[_leftBytes.Length + vRawData.Length];
                Array.Copy(_leftBytes, 0, newLeftBytes, 0, _leftBytes.Length);
                Array.Copy(vRawData, 0, newLeftBytes, _leftBytes.Length, vRawData.Length);
                _leftBytes = newLeftBytes;
            }
        }
        else
        {
            _leftBytes = vRawData;
        }

        __bodyLen = GetBodyLen(_leftBytes, 4);

        __leftLen = _leftBytes.Length - HeaderAndBodyLenOffset - __bodyLen;
        if (__leftLen == 0)
        {
            //刚好一个包长
            Logger.Log("刚好一个包长 总长:" + _leftBytes.Length + " 一个包长:" + (HeaderAndBodyLenOffset + __bodyLen));
            Parser(_leftBytes, __bodyLen);

            //没有包要粘了
            _leftBytes = null;
        }
        else if(__leftLen > 0)
        {
            Logger.Log("大于一个包长 总长:" + _leftBytes.Length + " 一个包长:" + (HeaderAndBodyLenOffset + __bodyLen));
            Parser(_leftBytes, __bodyLen);

            //剩下的存起来,有包要粘了
            byte[] newLeftBytes = new byte[__leftLen];
            Array.Copy(_leftBytes, HeaderAndBodyLenOffset + __bodyLen, newLeftBytes, 0, __leftLen);
            _leftBytes = newLeftBytes;

            //继续解析
            ParserRawData(null);
        }
        else
        {
            Logger.Log("小于一个包长 总长:" + _leftBytes.Length + " 一个包长:" + (HeaderAndBodyLenOffset + __bodyLen));
        }

    }

     void Parser(byte[] vData, int vBodyLen)
    {
        //双方都是以header通讯，因为它长度固定4
        string __header = Encoding.UTF8.GetString(vData, 0, HeaderOffset);
        __commandName = UtilityMsg.GetCommandNameByHeader(__header);

        if (vBodyLen > 0)
        {
            byte[] leftByte = new byte[vBodyLen];
            Array.Copy(vData, HeaderAndBodyLenOffset, leftByte, 0, vBodyLen);


            if (!_commandName2Type.TryGetValue(__commandName, out __type))
            {
                Debug.LogError("找不到命令号:" + __commandName + "对应的proto buff");
            }

            object proto = UtilityProbuff.DeSerialize(__type, leftByte);

            Logger.Log("<color=yellow>收到消息:</color>" + __commandName + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(proto));
            SendInfo sendInfo = UtilityObjectPool.Instance.Dequeue<SendInfo>();
            sendInfo.m_CommandName = __commandName;
            sendInfo.m_Info = proto;
            _recByteQueue.Enqueue(sendInfo);
        }
        else
        {
            Logger.Log("<color=yellow>收到没有包体的消息:</color>" + __commandName);
            SendInfo sendInfo = UtilityObjectPool.Instance.Dequeue<SendInfo>();
            sendInfo.m_CommandName = __commandName;
            sendInfo.m_Info = null;
            _recByteQueue.Enqueue(sendInfo);
        }

    }

    public static int GetBodyLen(byte[] vBytes, int vOffset)
    {
        return vBytes[vOffset] * 256 + vBytes[vOffset + 1];
    }

  
    public class SendInfo
    {
        public CommandName m_CommandName;
        public object m_Info;
    }
    private Queue<SendInfo> _recByteQueue = new Queue<SendInfo>();
    public void OnUpdate()
    {
        if(_recByteQueue.Count > 0)
        {
            SendInfo info = _recByteQueue.Dequeue();
            Facade.Instance.SendCommand(info.m_CommandName, info.m_Info);
            UtilityObjectPool.Instance.Enqueue<SendInfo>(info);
        }

        if(_socketStateQueue.Count > 0)
        {
            SocketState state = _socketStateQueue.Dequeue();
            switch (state)
            {
                case SocketState.None:
                    break;
                case SocketState.Connected:
                    Facade.Instance.SendEvent(GameEvent.UI_ShowTinyTip, "Net Connected");
                    Facade.Instance.SendEvent(GameEvent.Socket_Connected);
                    break;
                case SocketState.Disconnected:
                    TipManager.Instance.ShowCommonTip( "Lost Net", (TipButtonType vType)=>{ SocketManager.Instance.Connect(); });
                    Facade.Instance.SendEvent(GameEvent.Socket_Disconnected);
                    break;
                default:
                    break;
            }
        }
    }

    //
    private Queue<SocketState> _socketStateQueue = new Queue<SocketState>();
    public void EnqueueSocketState(SocketState vSocketState)
    {
        _socketStateQueue.Enqueue(vSocketState);
    }
}

public enum SocketState
{
    None,
    Connected,
    Disconnected
}
