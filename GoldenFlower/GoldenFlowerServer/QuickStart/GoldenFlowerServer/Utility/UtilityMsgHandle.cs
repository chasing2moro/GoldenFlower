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
}

