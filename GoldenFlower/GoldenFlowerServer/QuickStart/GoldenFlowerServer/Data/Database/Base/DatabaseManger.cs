using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class DatabaseManger
{
    public static bool IsUserExist(string vUserName)
    {
        //玩家名字是否有
        List<DataBaseUser> dataBaseUser = UtilityDataBase.Instance.ReadTable<DataBaseUser>(DataBaseUser.GetTableName(),
            new string[] { "username" },
            new string[] { "=" },
            new string[] { "'" + vUserName + "'"});
        if (dataBaseUser.IsNullOrEmpty())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool IsUserExist(string vUserName, string vPassword, out int vUserId)
    {
        //玩家名字&密码是否正确
        List<DataBaseUser> dataBaseUser = UtilityDataBase.Instance.ReadTable<DataBaseUser>(DataBaseUser.GetTableName(),
            new string[] { "username", "password" },
            new string[] { "=", "=" },
            new string[] { "'" + vUserName + "'", "'" + vPassword + "'"});
        if (dataBaseUser.IsNullOrEmpty())
        {
            vUserId = 0;
            return false;
        }
        else
        {
            vUserId = dataBaseUser[0].id;
            return true;
        }
    }

    public static DataBaseReource GetUserResource(int vPlayerId)
    {
        DataBaseReource dataBaseReource = UtilityDataBase.Instance.ReadTableFirst<DataBaseReource>(DataBaseReource.GetTableName(),
                                            "userid",
                                            "=",
                                            vPlayerId.ToString());
        return dataBaseReource;
    }
}

