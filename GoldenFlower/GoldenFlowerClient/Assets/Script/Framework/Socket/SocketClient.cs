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
    
    private Queue<byte[]> _recByteQueue;
    private SocketParser _parser;
    void Awake()
    {
        _recByteQueue = new Queue<byte[]>();
        _parser = new SocketParser();
    }

    void Update()
    {
        if(_recByteQueue.Count > 0)
        {
           byte[] recByte = _recByteQueue.Dequeue();

#if true
            string str = "";
            for (int i = 0; i < recByte.Length; i++)
            {
                str += ":" + recByte[i];
            }
            Debug.Log("recv" + str);
#endif
            //解析网络byte
            _parser.Parser(recByte);
        }
    }

    public string m_IP = "127.0.0.1";
    public int m_Port = 8000;
    private Socket socket;
    private IAsyncResult recAsyncResult;
    public void Connect()
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

    public void Send(byte[] data)
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

                //缓存起来
                _recByteQueue.Enqueue(trueByte);
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

