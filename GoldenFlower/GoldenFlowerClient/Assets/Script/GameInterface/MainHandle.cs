using UnityEngine;
using System.Collections;

public class MainHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Facade.Instance.RegistCommand(CommandName.UPDATERESOURCE, OnHandleUpdateResource);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.UPDATERESOURCE, OnHandleUpdateResource);
    }

    object OnHandleUpdateResource(params object[] args)
    {
        defaultproto.UpdateResource proto = args[0] as defaultproto.UpdateResource;
        DataManagerPlayer.Instance.OnHandleUpdateResource(proto);

        return null; 
    }
}
