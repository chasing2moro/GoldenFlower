using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 此类 测试使用
/// </summary>
public class ProtoRecipe : MonoBehaviour {
	//public List<RecordRecipe> m_RecordRecipeList;
    public RecordRecipe recordRecipe1;
    public RecordRecipe recordRecipe2;
    // Use this for initialization
    void Start () {
        XMLReader.CacheRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath());
        XMLReader.CacheRecord<int, RecordRecipe>("recipe1");
    }
	
    [ContextMenu("Read")]
    void Read()
    {
        recordRecipe1 = XMLReader.GetRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath(), 2);
        recordRecipe2 = XMLReader.GetRecord<int, RecordRecipe>("recipe1", 5);
    }
	// Update is called once per frame
	void Update () {
	
	}

 
}
