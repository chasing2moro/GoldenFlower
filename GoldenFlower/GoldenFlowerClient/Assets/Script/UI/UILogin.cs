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
        Facade.Instance.RegistCommand(CommandName.REGISTERACCOUNT, OnHandleCommand);
    }

    void OnDisable()
    {
        Facade.Instance.RegistCommand(CommandName.REGISTERACCOUNT, OnHandleCommand);
    }

    object OnHandleCommand(params object[] args)
    {
        defaultproto.RepRegisterAcount rep = args[0] as defaultproto.RepRegisterAcount;
        m_TextTips.text = "result:" + rep.result.errorCode + " : " + rep.result.errorDes;
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
