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
    public static void AssignErrorDes(object vProto, string vErrorDes)
    {
#if true
        Type type = vProto.GetType();
       PropertyInfo property = type.GetProperty("errorDes");
        property.SetValue(vProto, vErrorDes, null);
#endif
    }
}

