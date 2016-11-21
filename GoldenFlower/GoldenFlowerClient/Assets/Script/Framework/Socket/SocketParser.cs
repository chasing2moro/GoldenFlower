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
    int __leftLen;//剩余字节
    CommandName __commandName;//命令号
    Type __type;//proto buff 类
    Dictionary<CommandName, Type> _commandName2Type = new Dictionary<CommandName, Type>()
   {
       {CommandName.ADD, typeof(defaultproto.account) },
       {CommandName.ECHO, typeof(defaultproto.account) },
       {CommandName.MUTL, typeof(defaultproto.account) },
        {CommandName.REGISTERACCOUNT, typeof(defaultproto.RepRegisterAcount) },
   };
    public void Parser(byte[] vData)
    {
        //双方都是以header通讯，因为它长度固定4
        string __header = Encoding.UTF8.GetString(vData, 0, 4);
        __commandName = UtilityMsg.GetCommandNameByHeader(__header);

        //剩余的字节
        __leftLen = vData.Length - 4;
        byte[] leftByte = new byte[__leftLen];
        Array.Copy(vData, 4, leftByte, 0, __leftLen);


        if (!_commandName2Type.TryGetValue(__commandName, out __type))
        {
            Debug.LogError("找不到命令号:" + __commandName + "对应的proto buff");
        }

        object proto = UtilityProbuff.DeSerialize(__type, leftByte);

        Logger.Log("<color=yellow>收到消息:</color>" + Newtonsoft.Json.JsonConvert.SerializeObject(proto));
        //Debug.Log(proto.ToString());
     
        Facade.Instance.SendCommand(__commandName, proto);
    }
}
