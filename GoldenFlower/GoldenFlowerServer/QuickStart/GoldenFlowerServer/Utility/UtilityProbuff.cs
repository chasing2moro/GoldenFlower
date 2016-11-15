﻿using ProtoBuf;
using System;
using System.Collections.Generic;

public class UtilityProbuff
{
    static ProtoBuf.Meta.RuntimeTypeModel serializer;

    //获取解析器
    public static ProtoBuf.Meta.RuntimeTypeModel GetSerializer()
    {
        if (serializer == null)
            serializer = ProbuffProtocolSerializer.Create();
        return serializer;
    }

    public static byte[] Serialize(IExtensible vProto)
    {
        byte[] data;
        System.IO.MemoryStream s1 = new System.IO.MemoryStream();
        GetSerializer().Serialize(s1, vProto);
        data = new byte[s1.Length];
        //不能直接使用s1.GetBuffer()，否则会因为数据包长度不正确而导致消息解析失败。
        Array.Copy(s1.GetBuffer(), data, s1.Length);
        return data;
    }

    public static T DeSerialize<T>(byte[] vData) where T : new()
    {
        System.IO.MemoryStream s1 = new System.IO.MemoryStream(vData);
        T proto = new T();
        GetSerializer().Deserialize(s1, proto, typeof(T));

        return proto;
    }
}
