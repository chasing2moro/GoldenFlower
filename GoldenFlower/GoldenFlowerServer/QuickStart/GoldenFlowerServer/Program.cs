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
            string dataSource = string.Format("data source={0}", "../Sqlite/goldenflower.db");
            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    DataTable dt = sh.GetTableStatus();

                    DataTable dt1 = sh.GetTableList();

                    DataTable dt2 = sh.Select("select * from config_net");
                    string str = "";
                    foreach (DataRow dr in dt2.Rows)
                    {
                        foreach (DataColumn dc in dt2.Columns)
                        {
                            str += ":" + dr[dc];
                        }
                    }
                    Console.Write(str);
                    conn.Close();
                }
            }
            Console.ReadKey();
            return;
            System.Console.ReadKey();
            CustomProtocolServerTest test = new CustomProtocolServerTest();
            test.Setup();
            test.StartServer();
            Console.ReadKey();
        }
       }
}
