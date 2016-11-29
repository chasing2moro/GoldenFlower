using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBattle : UIBase
{
 
    public Text m_TextTips;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Facade.Instance.RegistCommand(CommandName.JOININBATTLE, OnHandleJoinInBattle);
        Facade.Instance.RegistCommand(CommandName.BET, OnHandleBet);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.JOININBATTLE, OnHandleJoinInBattle);
        Facade.Instance.UnRegistCommand(CommandName.BET, OnHandleBet);
    }

    object OnHandleJoinInBattle(params object[] args)
    {

        return null;
    }

    object OnHandleBet(params object[] args)
    {

        return null;
    }

    public void OnButtonReqJoinBattleClicked()
    {
        defaultproto.ReqJoinBattle vProto = new defaultproto.ReqJoinBattle();
        vProto.roomId = 1;

        SocketManager.Instance.SendMsg(CommandName.JOININBATTLE, vProto);
    }

    public void OnButtonReqBetClicked()
    {
        defaultproto.ReqBet reqBet = new defaultproto.ReqBet();
        reqBet.count = 20;
        SocketManager.Instance.SendMsg(CommandName.BET, reqBet);
    }

    public void OnButtonReqQuitClicked()
    {
        defaultproto.ReqQuit vProto = new defaultproto.ReqQuit();
        SocketManager.Instance.SendMsg(CommandName.QUIT, vProto);
    }

}
