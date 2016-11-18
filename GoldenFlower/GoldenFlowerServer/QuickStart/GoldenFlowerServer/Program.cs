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
#if true
            Logger.Log("sqlite测试");
            UtilityDataBase.Instance.ConnectDatabase();
            List<DatabaseServer> servers = UtilityDataBase.Instance.ReadFullTable<DatabaseServer>("config_net");
            foreach (var item in servers)
            {
                Console.WriteLine(item.ip + " " + item.port);
            }

            servers = UtilityDataBase.Instance.ReadTable<DatabaseServer>("config_net", new string[] { "ip", "port" }, new string[] { "port" }, new string[] { "=" }, new string[] { "900" });
            foreach (var item in servers)
            {
                Console.WriteLine(item.ip + " - " + item.port);
            }
#endif



#if true
            Logger.Log("配置表测试");
            UtilityProto.CacheAllRecord();
            RecordRecipe recordRecipe1 = UtilityProto.GetRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath(), 2);
            RecordRecipe recordRecipe2 = UtilityProto.GetRecord<int, RecordRecipe>("recipe1", 5);
            Logger.Log(recordRecipe1.id + " "+ recordRecipe1.preptime + " " + recordRecipe1.recipename + " " + recordRecipe1.type);
            Logger.Log(recordRecipe2.id + " " + recordRecipe2.preptime + " " + recordRecipe2.recipename + " " + recordRecipe2.type);
#endif

            Logger.Log("服务器测试");
            CustomProtocolServerTest test = new CustomProtocolServerTest();
            test.Setup();
            test.StartServer();
            Console.ReadKey();
        }
       }
}
