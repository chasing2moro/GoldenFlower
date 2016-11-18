using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class DataBaseUser : DatabaseRecord
{
    /// <summary>
    /// 数据表的名字
    /// </summary>
    /// <returns></returns>
    public static string GetTableName()
    {
        return "user";
    }

    [DatabaseAttributeInt]
    public int id;

    [DatabaseAttributeString]
    public string username;

    [DatabaseAttributeString]
    public string password;
}

