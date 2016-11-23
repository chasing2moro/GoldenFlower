using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


public class UtilityMsgHandle
{
    /// <summary>
    /// 错误码赋值，后面可以关闭
    /// </summary>
    /// <param name="vProtoErrorDes"></param>
    /// <param name="vErrorDes"></param>
    public static void AssignErrorDes(object vProto, defaultproto.ErrorCode vErrorCode, string vErrorDes = null)
    {
#if true
        PropertyInfo propertyInfo = vProto.GetType().GetProperty("result");
        defaultproto.Result result = propertyInfo.GetValue(vProto, null) as defaultproto.Result;
        if (result == null)
        {
            result = new defaultproto.Result();
            propertyInfo.SetValue(vProto, result, null);
        }

        result.errorCode = vErrorCode;

        if (!string.IsNullOrEmpty(vErrorDes))
            result.errorDes = vErrorDes;
        else
            result.errorDes = null;
       // Type type = vProto.GetType();
       //PropertyInfo property = type.GetProperty("errorDes");
       // property.SetValue(vProto, vErrorDes, null);
#endif
    }


    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="vCommandName"></param>
    /// <param name="vProto"></param>
    /// <param name="vPlayerIds"></param>
    public static void BrocastMsgWithPlayerIds(CommandName vCommandName,  IExtensible vProto, params int[] vPlayerIds)
    {
        if (!vPlayerIds.IsNullOrEmpty())
        {
            for (int i = 0; i < vPlayerIds.Length; i++)
            {
                CustomProtocolSession session = PlayerDataManager.Instance.GetSession(vPlayerIds[i]);
                if(session != null)
                {
                    session.SendProto(vCommandName, vProto);
                }
            }
        }
    }


    /// <summary>
    /// 针对玩家id发消息
    /// </summary>
    /// <param name="vCommandName"></param>
    /// <param name="vProto"></param>
    /// <param name="vPlayerId"></param>
    public static void SendMsgWithPlayerId(CommandName vCommandName, IExtensible vProto, int vPlayerId)
    {
        CustomProtocolSession session = PlayerDataManager.Instance.GetSession(vPlayerId);
        if (session != null)
        {
            session.SendProto(vCommandName, vProto);
        }
    }
}

