#define kTest
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace SuperSocket.QuickStart.TelnetServer
{
    class Program
    {

        static void Main(string[] args)
        {
            UtilityDataBase.Instance.ConnectDatabase();
            List<DatabaseServer> servers = UtilityDataBase.Instance.ReadFullTable<DatabaseServer>("config_net");
            foreach (var item in servers)
            {
                Console.WriteLine(item.ip + " " + item.port);
            }

            Console.ReadKey();
            return;
     
            CustomProtocolServerTest test = new CustomProtocolServerTest();
            test.Setup();
            test.StartServer();
            Console.ReadKey();
        }
       }
}
