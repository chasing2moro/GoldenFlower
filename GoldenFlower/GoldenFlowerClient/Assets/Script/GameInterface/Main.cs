using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

    void Awake()
    {
        //注册所有单例
        UtilityInterface.StartUp();

        //连接socket
        SocketManager.Instance.Connect();
    }
	// Use this for initialization
	void Start () {
        UIManager.Instance.ShowUI(UIType.UILogin);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //public RecordRecipe recordRecipe1;
    //public RecordRecipe recordRecipe2;
    //[ContextMenu("ReadXML")]
    //public void ReadXML()
    //{
    //    UtilityProto.CacheAllRecord();

    //    recordRecipe1 = UtilityProto.GetRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath(), 2);
    //    recordRecipe2 = UtilityProto.GetRecord<int, RecordRecipe>("recipe1", 5);
    //}

}
