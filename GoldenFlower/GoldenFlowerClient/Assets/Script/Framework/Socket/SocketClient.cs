using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/*
 * 
 *Socket客户端通信类
 * 
 * lm
 */
public class SocketClient : MonoBehaviour
{
    [ContextMenu("Connect")]
    void _connect()
    {
        string header = UtilityMsg.GetHeaderByCommandName(CommandName.ECHO);
        Debug.Log("header:" + header);

        string command = UtilityMsg.GetCommandNameByHeader(header);
        Debug.Log("command:" + command);

         Connect();
    }

    [ContextMenu("Send")]
    void _send()
    {
       
        //socketStream.Write(requestNameData, 0, requestNameData.Length);
        //var data = Encoding.ASCII.GetBytes(currentMessage);
       // #region Probuff
        ProtoBuf.Meta.RuntimeTypeModel serializer = ProbuffProtocolSerializer.Create();

        defaultproto.example protoexample = new defaultproto.example();
        protoexample.name = "name1";
        protoexample.field.Add(1);
        protoexample.field.Add(2);
        protoexample.gender = 1;
        protoexample.year = 30;


        System.IO.MemoryStream stream = new System.IO.MemoryStream();


        //包名长
        // Debug.Log("包名 长度：" + backageName.Length);
        //  if (backageName.Length > byte.MaxValue)
        //     Debug.LogError("报名长度只支持255，你的包名长度：" + backageName.Length);
        // stream.WriteByte((byte)backageName.Length);

        //包名
        byte[] backageName = Encoding.ASCII.GetBytes(UtilityMsg.GetHeaderByCommandName(CommandName.ADD));
        Debug.Log("包名 长度：" + backageName.Length);
        stream.Write(backageName, 0, backageName.Length);

        byte[] backageBody = UtilityProbuff.Serialize(protoexample);

        //包体长
        Debug.Log("包体Header 长度：" + 4);
        stream.Write(new byte[] { (byte)(backageBody.Length / 256), (byte)(backageBody.Length % 256) }, 0, 2);

        //包体
        Debug.Log("包体 长度：" + backageBody.Length);
        stream.Write(backageBody, 0, backageBody.Length);


        byte[] sendbyte = new byte[stream.Length];
        Array.Copy(stream.GetBuffer(), sendbyte, stream.Length);

        Debug.Log("整包 长度：" + sendbyte.Length);
        string str = "";
        for (int i = 0; i < sendbyte.Length; i++)
        {
            str += "[" + i + "]:" + sendbyte[i];
        }
        Debug.Log("send" + str);

        Send(sendbyte);
    }

    void _recv()
    {

    }

    public string m_IP = "127.0.0.1";
    public int m_Port = 8000;
    private Socket socket;
    private IAsyncResult recAsyncResult;
    void Connect()
    {

        //采用TCP方式连接
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //服务器IP地址
        IPAddress address = IPAddress.Parse(m_IP);

        //服务器端口
        IPEndPoint endpoint = new IPEndPoint(address, m_Port);

        //异步连接,连接成功调用connectCallback方法
        IAsyncResult connectAsyncResult = socket.BeginConnect(endpoint, new AsyncCallback(OnConnected), socket);

        //这里做一个超时的监测，当连接超过5秒还没成功表示超时
        bool success = connectAsyncResult.AsyncWaitHandle.WaitOne(5000, true);
        if (!success)
        {
            //超时
            Close();
            Debug.Log("connect Time Out");
        }
    }

    protected void Recv()
    {
        try
        {
            byte[] buf = new byte[0x1000];
            this.recAsyncResult = socket.BeginReceive(buf, 0, 0x1000, SocketFlags.None, new AsyncCallback(this.OnRecv), buf);
        }
        catch (SocketException e)
        {
            this.Close(e.ErrorCode);
        }
    }

    protected void Send(byte[] data)
    {
        try
        {
            IAsyncResult sendAsyncResult = socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSend), socket);

            //超过5s服务器不响应
            bool success = sendAsyncResult.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                socket.Close();
                Debug.Log("服务器不响应客户端消息，可能断网了");
            }
        }
        catch (SocketException e)
        {
            this.Close(e.ErrorCode);
        }
    }


    /// <summary>
    /// socket 连接成功
    /// </summary>
    private void OnConnected(IAsyncResult asyncConnect)
    {
        Debug.Log("connect success");
        //开始接收Socket消息
        Recv();
    }

    /// <summary>
    /// socket 收到字节流
    /// </summary>
    private void OnRecv(IAsyncResult vIAsyncResult)
    {
        try
        {
            //不要再侦听超时了
            vIAsyncResult.AsyncWaitHandle.Close();

            //回调的最后的参数传进来的
            //socket 字节流
            byte[] buf = (byte[])vIAsyncResult.AsyncState;
            //socket 字节流长度
            int len = socket.EndReceive(vIAsyncResult);
            this.recAsyncResult = null;
            if (len > 0)
            {

                byte[] trueByte = new byte[len];
                Array.Copy(buf, trueByte, len);

                string str = "";
                for (int i = 0; i < trueByte.Length; i++)
                {
                    str += ":" + trueByte[i];
                }
                Debug.Log("recv" + str);


                // defaultproto.example protoExample = UtilityProbuff.DeSerialize<defaultproto.example>(trueByte);
                // Debug.Log(protoExample.name);
                // Debug.Log(protoExample.field[0]);
                // Debug.Log(protoExample.field[1]);
                // Debug.Log(protoExample.gender);
                // Debug.Log(protoExample.year);

                defaultproto.account protoAcount = UtilityProbuff.DeSerialize<defaultproto.account>(trueByte);
                Debug.Log(protoAcount.name);
            }


            //此buf实际上 一直重用
            socket.BeginReceive(buf, 0, 0x1000, SocketFlags.None, new AsyncCallback(OnRecv), buf);
        }
        catch (SocketException e)
        {
            this.Close(e.ErrorCode);
        }
    }

    private void OnSend(IAsyncResult vIAsyncResult)
    {
        try
        {
            //不要再侦听超时了
            vIAsyncResult.AsyncWaitHandle.Close();
            ((Socket)vIAsyncResult.AsyncState).EndSend(vIAsyncResult);
        }
        catch (SocketException e)
        {
            this.Close(e.ErrorCode);
        }
    }
    
    //关闭Socket
    public void Close(int vErrorCode = -100)
    {
        Debug.LogError("socket close" + vErrorCode);
        if (socket != null && socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        socket = null;
    }

}

