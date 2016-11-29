using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILogin : UIBase
{
    public InputField m_InputFieldUserName;
    public InputField m_InputFieldPassword;
    public Text m_TextTips;
    public enum State
    {
        None,
        Register,
        Login
    }
    public State m_State;
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
        Facade.Instance.RegistEvent(GameEvent.Socket_Connected, OnHandleSocketConnected);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.REGISTERACCOUNT, OnHandleCommandRegister);
        Facade.Instance.UnRegistCommand(CommandName.LOGIN, OnHandleCommandLogin);
        Facade.Instance.UnRegistEvent(GameEvent.Socket_Connected, OnHandleSocketConnected);
    }

    object OnHandleCommandRegister(params object[] args)
    {
        defaultproto.RepRegisterAcount rep = args[0] as defaultproto.RepRegisterAcount;
        m_TextTips.text = "result:" + rep.result.errorCode + " : " + rep.result.errorDes;
        if (rep.result.errorCode == defaultproto.ErrorCode.None)
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

    object OnHandleSocketConnected(params object[] args)
    {

        switch (m_State)
        {
            case State.None:
                break;
            case State.Register:
                defaultproto.ReqRegisterAcount vProto = new defaultproto.ReqRegisterAcount();
                vProto.username = m_InputFieldUserName.text.Trim();
                vProto.password = m_InputFieldPassword.text.Trim();

                SocketManager.Instance.SendMsg(CommandName.REGISTERACCOUNT, vProto);
                break;
            case State.Login:
                defaultproto.ReqLogin proto = new defaultproto.ReqLogin();
                proto.username = m_InputFieldUserName.text.Trim();
                proto.password = m_InputFieldPassword.text.Trim();

                SocketManager.Instance.SendMsg(CommandName.LOGIN, proto);
                break;
            default:
                break;
        }
        return null;
    }

    public void OnButtonRegisterClicked()
    {
        m_State = State.Register;

        //连接socket
        SocketManager.Instance.Connect();
    }

    public void OnButtonLoginClicked()
    {
        m_State = State.Login;

        //连接socket
        SocketManager.Instance.Connect();
    }

}
