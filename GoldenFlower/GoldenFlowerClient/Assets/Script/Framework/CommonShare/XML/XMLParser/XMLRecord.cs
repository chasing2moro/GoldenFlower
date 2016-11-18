using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Xml;
using System;

public abstract class XMLRecord
{
    //子类要实现它
    //public static string GetConfigPath()
    //{
    //    return null;
    //}

    //vKeyName 约定为 id
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

	public bool AssignMember(XmlElement vElement){
		FieldInfo[] fieldInfos = GetFieldInfos();
		foreach (FieldInfo field in fieldInfos) {
			XMLAttributeMember[] attributeMembers = (XMLAttributeMember[])field.GetCustomAttributes(typeof(XMLAttributeMember), true);
			if(attributeMembers.Length > 0){
				XMLAttributeMember attributeMember = attributeMembers[0];
				field.SetValue(this, attributeMember.GetData(vElement, field.Name));
			}
		}
		return false;
	}

}
