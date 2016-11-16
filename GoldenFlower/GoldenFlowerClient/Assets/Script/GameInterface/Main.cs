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
        Facade.Instance.RegistCommand(CommandName.ADD, OnHandleAddCommand);
        Facade.Instance.RegistCommand(CommandName.ECHO, OnHandleEchoCommand);
        Facade.Instance.RegistCommand(CommandName.MUTL, OnHandleEchoCommand);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.ADD, OnHandleAddCommand);
        Facade.Instance.UnRegistCommand(CommandName.ECHO, OnHandleEchoCommand);
        Facade.Instance.RegistCommand(CommandName.MUTL, OnHandleEchoCommand);
    }

    object OnHandleAddCommand(params object[] args)
    {
        Debug.Log((args[0] as defaultproto.account).name);
        return null;
    }

    object OnHandleEchoCommand(params object[] args)
    {
        Debug.Log((args[0] as defaultproto.account).name);
        return null;
    }
}
