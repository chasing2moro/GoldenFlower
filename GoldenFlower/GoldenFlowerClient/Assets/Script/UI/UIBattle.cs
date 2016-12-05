using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBattle : UIBase
{
 
    public Text m_TextTips;
    //玩家加入的预设
    public GameObject m_StuffChar;

    public Transform[] m_Pos;
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
        Transform spawnRoot = null;
        foreach (Transform pos in m_Pos)
        {
            if(pos.transform.childCount == 0)
            {
                spawnRoot = pos;
                break;
            }
        }
        if (spawnRoot == null)
            Debug.LogError("出错，找不到位置");

        GameObject gameObject = Instantiate(m_StuffChar);
        gameObject.transform.SetParent(spawnRoot, false);
        gameObject.transform.localPosition = Vector3.zero;

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
        SocketManager.Instance.SendMsg(CommandName.QUIT, null);
    }

}
