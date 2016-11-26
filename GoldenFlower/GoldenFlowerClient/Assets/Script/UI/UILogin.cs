using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase
{
    public InputField m_InputFieldUserName;
    public InputField m_InputFieldPassword;
    public Text m_TextTips;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Facade.Instance.RegistCommand(CommandName.REGISTERACCOUNT, OnHandleCommandRegister);
        Facade.Instance.RegistCommand(CommandName.LOGIN, OnHandleCommandLogin);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.REGISTERACCOUNT, OnHandleCommandRegister);
        Facade.Instance.UnRegistCommand(CommandName.LOGIN, OnHandleCommandLogin);
    }

    object OnHandleCommandRegister(params object[] args)
    {
        defaultproto.RepRegisterAcount rep = args[0] as defaultproto.RepRegisterAcount;
        m_TextTips.text = "result:" + rep.result.errorCode + " : " + rep.result.errorDes;
        if(rep.result.errorCode == defaultproto.ErrorCode.None)
        {
            DataManagerPlayer.Instance.m_UserName = m_InputFieldUserName.text.Trim();
            DataManagerPlayer.Instance.m_PlayerId = rep.playerId;

            SceneManager.Instance.LoadScene(SceneType.Battle);
        }
        return null;
    }

    object OnHandleCommandLogin(params object[] args)
    {
        defaultproto.RepLogin rep = args[0] as defaultproto.RepLogin;
        m_TextTips.text = "result:" + rep.result.errorCode + " : " + rep.result.errorDes;
        if (rep.result.errorCode == defaultproto.ErrorCode.None)
        {
            DataManagerPlayer.Instance.m_UserName = m_InputFieldUserName.text.Trim();
            DataManagerPlayer.Instance.m_PlayerId = rep.playerId;

            SceneManager.Instance.LoadScene(SceneType.Battle);
        }
        return null;
    }

    public void OnButtonRegisterClicked()
    {
        defaultproto.ReqRegisterAcount vProto = new defaultproto.ReqRegisterAcount();
        vProto.username = m_InputFieldUserName.text.Trim();
        vProto.password = m_InputFieldPassword.text.Trim();

        SocketManager.Instance.SendMsg(CommandName.REGISTERACCOUNT, vProto);
    }

    public void OnButtonLoginClicked()
    {
        defaultproto.ReqLogin vProto = new defaultproto.ReqLogin();
        vProto.username = m_InputFieldUserName.text.Trim();
        vProto.password = m_InputFieldPassword.text.Trim();

        SocketManager.Instance.SendMsg(CommandName.LOGIN, vProto);
    }

}
