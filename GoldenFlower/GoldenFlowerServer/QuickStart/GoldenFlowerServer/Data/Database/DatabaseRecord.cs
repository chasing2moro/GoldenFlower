using System.Collections;
using System.Reflection;
using System.Xml;
using System;
using System.Data;

public abstract class DatabaseRecord
{
    public virtual T GetKey<T>(string vKeyName)
    {
        FieldInfo fieldInfo = GetType().GetField(vKeyName);
        object id = fieldInfo.GetValue(this);
        return (T)Convert.ChangeType(id, typeof(T));
    }

    public FieldInfo[] GetFieldInfos(){
		return GetType().GetFields(BindingFlags.Public|
		                               BindingFlags.NonPublic|
		                               BindingFlags.Instance|
		                               BindingFlags.GetProperty);
	}

	public bool AssignMember(DataRow vRow, DataColumnCollection vColumns)
    {
		FieldInfo[] fieldInfos = GetFieldInfos();
		foreach (FieldInfo field in fieldInfos) {
            DatabaseAttributeMember[] attributeMembers = (DatabaseAttributeMember[])field.GetCustomAttributes(typeof(DatabaseAttributeMember), true);
			if(attributeMembers.Length > 0){
                DatabaseAttributeMember attributeMember = attributeMembers[0];
				field.SetValue(this, attributeMember.GetData(vRow, vColumns, field.Name));
			}
		}
		return false;
	}

}
