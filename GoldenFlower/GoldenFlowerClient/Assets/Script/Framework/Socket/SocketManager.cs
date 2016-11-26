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


    [ContextMenu("Connect")]
    public void Connect()
    {
        m_SocketClient.Connect();
    }

    [ContextMenu("Register")]
     void OnButtonRegisterClicked()
    {
        defaultproto.ReqRegisterAcount vProto = new defaultproto.ReqRegisterAcount();
        vProto.username = "123bx";
        vProto.password = "123";

        SocketManager.Instance.SendMsg(CommandName.REGISTERACCOUNT, vProto);
    }


    //defaultproto.example vProto = new defaultproto.example();
    //  vProto.name = VCommandName.ToString();
    //  vProto.field.Add(1);
    //  vProto.field.Add(2);
    //  vProto.gender = 1;
    // vProto.year = 30;
    string __strLog;
    /// <summary>
    /// 发送消息给服务器
    /// </summary>
    /// <param name="vCommandName">服务器命令</param>
    /// <param name="vProto">Probuff</param>
    public void SendMsg(CommandName vCommandName, IExtensible vProto)
    {
        __strLog = Newtonsoft.Json.JsonConvert.SerializeObject(vProto);
        Logger.Log("<color=green>发送消息:</color>：" + vCommandName + "" + __strLog);

        System.IO.MemoryStream stream = new System.IO.MemoryStream();

        //包名
        byte[] backageName = Encoding.UTF8.GetBytes(UtilityMsg.GetHeaderByCommandName(vCommandName));
        //__strLog = "包名长度：" + backageName.Length;
        stream.Write(backageName, 0, backageName.Length);

        if(vProto != null)
        {
            byte[] backageBody = UtilityProbuff.Serialize(vProto);

            //包体长
            //__strLog += " 包体Header长度：" + 2;
            stream.Write(new byte[] { (byte)(backageBody.Length / 256), (byte)(backageBody.Length % 256) }, 0, 2);

            //包体
            //__strLog += " 包体长度：" + backageBody.Length;
            stream.Write(backageBody, 0, backageBody.Length);
        }

        //stream 序列到 byte[]
        byte[] sendbyte = new byte[stream.Length];
        Array.Copy(stream.GetBuffer(), sendbyte, stream.Length);

        __strLog = "";
        for (int i = 0; i < sendbyte.Length; i++)
        {
            __strLog += " [" + i + ":" + sendbyte[i]+ "]";
        }
        Debug.Log("send("+ sendbyte.Length+ ")" + __strLog);

        m_SocketClient.Send(sendbyte);
    }
}
