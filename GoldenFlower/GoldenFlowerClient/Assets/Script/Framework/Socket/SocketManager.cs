using UnityEngine;
using System.Collections;
using ProtoBuf;
using System.Text;
using System;

public class SocketManager : MonoBehaviour
{
    public static SocketManager Instance;
    public SocketClient m_SocketClient;
    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Connect()
    {
        m_SocketClient.Connect();
    }




    //defaultproto.example vProto = new defaultproto.example();
    //  vProto.name = VCommandName.ToString();
    //  vProto.field.Add(1);
    //  vProto.field.Add(2);
    //  vProto.gender = 1;
    // vProto.year = 30;

    /// <summary>
    /// 发送消息给服务器
    /// </summary>
    /// <param name="vCommandName">服务器命令</param>
    /// <param name="vProto">Probuff</param>
    public void SendMsg(CommandName vCommandName, IExtensible vProto)
    {
        System.IO.MemoryStream stream = new System.IO.MemoryStream();

        //包名
        byte[] backageName = Encoding.UTF8.GetBytes(UtilityMsg.GetHeaderByCommandName(vCommandName));
        Debug.Log("包名 长度：" + backageName.Length);
        stream.Write(backageName, 0, backageName.Length);

        byte[] backageBody = UtilityProbuff.Serialize(vProto);

        //包体长
        Debug.Log("包体Header 长度：" + 2);
        stream.Write(new byte[] { (byte)(backageBody.Length / 256), (byte)(backageBody.Length % 256) }, 0, 2);

        //包体
        Debug.Log("包体 长度：" + backageBody.Length);
        stream.Write(backageBody, 0, backageBody.Length);

        //stream 序列到 byte[]
        byte[] sendbyte = new byte[stream.Length];
        Array.Copy(stream.GetBuffer(), sendbyte, stream.Length);

        Debug.Log("整包 长度：" + sendbyte.Length);
        string str = "";
        for (int i = 0; i < sendbyte.Length; i++)
        {
            str += "[" + i + "]:" + sendbyte[i];
        }
        Debug.Log("send" + str);

        m_SocketClient.Send(sendbyte);
    }
}
