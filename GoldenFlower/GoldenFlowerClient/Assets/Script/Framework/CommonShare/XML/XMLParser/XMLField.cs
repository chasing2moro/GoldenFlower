using System;
using System.Xml;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public abstract class XMLAttributeMember : Attribute{
	public abstract object GetData(XmlElement vElement, string vName);
}

//int
public class XMLAttributeInt : XMLAttributeMember{
	public override object GetData (XmlElement vElement, string vName)
	{
		try {
			return int.Parse(vElement.GetAttribute(vName));
		} catch (Exception ex) {
			Debug.Log("ex:" + ex.Message + " " + vName + " value:" + vElement.GetAttribute(vName));
		}
		return null;
	}
}

//string
public class XMLAttributeString : XMLAttributeMember{
	public override object GetData (XmlElement vElement, string vName)
	{
		return vElement.GetAttribute(vName);
	}
}

//string
public class XMLAttributeStringArray : XMLAttributeMember
{
    public override object GetData(XmlElement vElement, string vName)
    {
        string stringRaw = vElement.GetAttribute(vName);
        string[] stringArray = stringRaw.Split(',');
        return stringArray;
    }
}

//float
public class XMLAttributeFloat : XMLAttributeMember{
	public override object GetData (XmlElement vElement, string vName)
	{
		return float.Parse(vElement.GetAttribute(vName));
	}
}

//enum
public class XMLAttributeEnum : XMLAttributeMember{
	public XMLAttributeEnum(Type vType){
		_type = vType;
	}
	Type _type;

	public override object GetData (XmlElement vElement, string vName)
	{
		return Enum.Parse(_type, vElement.GetAttribute(vName));
		//Enum.GetName(_type, value); Reverse method to get the string
	}
}