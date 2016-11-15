using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;


    public class CustomProtocolSession : AppSession<CustomProtocolSession, BinaryRequestInfo>
    {
        protected override void HandleException(Exception e)
        {

        }
    }

