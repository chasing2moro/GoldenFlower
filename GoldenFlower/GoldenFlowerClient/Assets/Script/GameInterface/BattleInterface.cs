using UnityEngine;
using System.Collections;

public class BattleInterface : InterfaceBase
{

	// Use this for initialization
	void Start () {
        UIManager.Instance.ShowUI(UIType.UIBattle);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
