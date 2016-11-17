using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;


public class UtilityDataBase
{
    static UtilityDataBase _instance;
    public static UtilityDataBase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UtilityDataBase();
            }
            return _instance;
        }
    }

    SQLiteHelper _sqliteHelper;
    public void ConnectDatabase()
    {
        string dataSource = string.Format("data source={0}", GameConfig.DataBasePath);
        //连接数据库
        SQLiteConnection conn = new SQLiteConnection(dataSource);
        conn.Open();
   
        SQLiteCommand cmd = new SQLiteCommand();
        cmd.Connection = conn;

        _sqliteHelper = new SQLiteHelper(cmd);
       }


    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    public List<T> ReadFullTable<T>(string tableName)  where T : DatabaseRecord , new()
    {
        List<T> list = new List<T>();
 
        DataTable dataTable = _sqliteHelper.Select("select * from " + tableName);
        foreach (DataRow dr in dataTable.Rows)
        {
            T database = new T();
            database.AssignMember(dr, dataTable.Columns);
            list.Add(database);
        }
        /*
                  foreach (DataRow dr in dt2.Rows)
                    {
                        foreach (DataColumn dc in dt2.Columns)
                        {
                            str += ":" + dr[dc];
                        }
                    }*/
        return list;
    }
}

