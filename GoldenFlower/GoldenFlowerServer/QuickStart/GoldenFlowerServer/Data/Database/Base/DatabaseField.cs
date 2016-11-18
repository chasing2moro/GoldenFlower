using System;
using System.Data;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public abstract class DatabaseAttributeMember : Attribute{
	public abstract object GetData(DataRow vRow, DataColumnCollection vColumns, string vName);
}

//int
public class DatabaseAttributeInt : DatabaseAttributeMember{
	public override object GetData(DataRow vRow, DataColumnCollection vColumns, string vName)
	{
        try
        {
            DataColumn column = vColumns[vName];
            return (int)vRow[column];
        }
        catch (Exception ex)
        {
            Console.WriteLine(" error  ex:" + ex.Message + " " + vName + " 找不到");
        }
        return null;
	}
}

//string
public class DatabaseAttributeString : DatabaseAttributeMember{
    public override object GetData(DataRow vRow, DataColumnCollection vColumns, string vName)
    {
        try
        {
            DataColumn column = vColumns[vName];
            return (string)vRow[column];
        }
        catch (Exception ex)
        {
            Console.WriteLine(" error  ex:" + ex.Message + " " + vName + " 找不到");
        }
        return null;
    }
}
#if false

//string
public class DatabaseAttributeStringArray : DatabaseAttributeMember
{
    public override object GetData(DatabaseElement vElement, string vName)
    {
        string stringRaw = vElement.GetAttribute(vName);
        string[] stringArray = stringRaw.Split(',');
        return stringArray;
    }
}

//float
public class DatabaseAttributeFloat : DatabaseAttributeMember{
	public override object GetData (DatabaseElement vElement, string vName)
	{
		return float.Parse(vElement.GetAttribute(vName));
	}
}

//enum
public class DatabaseAttributeEnum : DatabaseAttributeMember{
	public DatabaseAttributeEnum(Type vType){
		_type = vType;
	}
	Type _type;

	public override object GetData (DatabaseElement vElement, string vName)
	{
		return Enum.Parse(_type, vElement.GetAttribute(vName));
		//Enum.GetName(_type, value); Reverse method to get the string
	}

}
#endif