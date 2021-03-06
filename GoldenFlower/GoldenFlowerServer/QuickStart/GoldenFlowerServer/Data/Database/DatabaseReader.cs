﻿using System.Collections;
using System.Xml;
using System.Collections.Generic;


public class DatabaseReader {
	static List<T> GetRecordList<T>(string vXMLString) where T:DatabaseRecord, new(){
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(vXMLString);

        //read header
        List<T> recordList = new List<T>();
		XmlNodeList nodeList = doc.LastChild.ChildNodes;
		foreach (XmlNode node in nodeList) {
            T record = new T();
            record.AssignMember(node as XmlElement);
            recordList.Add(record);
        }


		return recordList;
	}

    static Dictionary<KEY, VALUE> GetRecordDic<KEY, VALUE>(string vXMLString, string vKeyName = "id") where VALUE : XMLRecord, new()
    {
        Dictionary<KEY, VALUE> dic = new Dictionary<KEY, VALUE>();
        List<VALUE> list = GetRecordList<VALUE>(vXMLString);
        for (int i = 0; i < list.Count; i++)
        {
            VALUE record = list[i];
            dic[record.GetKey<KEY>(vKeyName)] = record;
        }
        return dic;
    }

    static Dictionary<string, object> _path2Dic;
    public static void CacheRecord<KEY, VALUE>(string vPath) where VALUE : XMLRecord, new()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(vPath);
        Dictionary<KEY, VALUE> dic = GetRecordDic<KEY, VALUE>(textAsset.text);

        if (_path2Dic == null)
            _path2Dic = new Dictionary<string, object>();

        _path2Dic[vPath] = dic;
    }

    public static VALUE GetRecord<KEY, VALUE>(string vPath, KEY vKey) where VALUE : XMLRecord, new()
    {
        object dicObj = null;
        VALUE record = null;
        if (_path2Dic.TryGetValue(vPath, out dicObj))
        {
            Dictionary<KEY, VALUE> dic = (Dictionary<KEY, VALUE>)dicObj;
            if(dic.TryGetValue(vKey, out record))
            {
                //
            }
            else
            {
                Debug.LogError("Can not Find path:" + vPath + " key:" + vKey);
            }
        }
        else
        {
            Debug.LogError("Can not Find path:" + vPath);
        }


        return record;
    }
}
