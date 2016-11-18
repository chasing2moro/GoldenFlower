using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Facade.Instance.RegistCommand(CommandName.ADD, OnHandleCommand);
        Facade.Instance.RegistCommand(CommandName.ECHO, OnHandleCommand);
        Facade.Instance.RegistCommand(CommandName.MUTL, OnHandleCommand);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.ADD, OnHandleCommand);
        Facade.Instance.UnRegistCommand(CommandName.ECHO, OnHandleCommand);
        Facade.Instance.RegistCommand(CommandName.MUTL, OnHandleCommand);
    }

    object OnHandleCommand(params object[] args)
    {
        Debug.Log((args[0] as defaultproto.account).name);
        return null;
    }

    public RecordRecipe recordRecipe1;
    public RecordRecipe recordRecipe2;
    [ContextMenu("ReadXML")]
    public void ReadXML()
    {
        UtilityProto.CacheAllRecord();

        recordRecipe1 = UtilityProto.GetRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath(), 2);
        recordRecipe2 = UtilityProto.GetRecord<int, RecordRecipe>("recipe1", 5);
    }

}
