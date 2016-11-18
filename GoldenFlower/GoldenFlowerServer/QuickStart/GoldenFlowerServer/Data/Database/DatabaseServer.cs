using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DatabaseServer : DatabaseRecord
{

    /// <summary>
    /// 数据表的名字
    /// </summary>
    /// <returns></returns>
    public static string GetTableName()
    {
        return "config_net";
    }

    [DatabaseAttributeString]
    public string ip;

    [DatabaseAttributeInt]
    public int port;
}

