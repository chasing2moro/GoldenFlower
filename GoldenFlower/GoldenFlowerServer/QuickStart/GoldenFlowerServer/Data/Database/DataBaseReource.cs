using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class DataBaseReource : DatabaseRecord
{
    /// <summary>
    /// 数据表的名字
    /// </summary>
    /// <returns></returns>
    public static string GetTableName()
    {
        return "resource";
    }

    [DatabaseAttributeInt]
    public int userid;

    [DatabaseAttributeInt]
    public int moneny;

    [DatabaseAttributeInt]
    public int coin;
}

