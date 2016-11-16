using UnityEngine;
using System.Collections;
public delegate object RegistFunction(params object[] pSender);
public class Facade : MonoBehaviour {

	public static Facade Instance;
	void Awake()
	{
        Instance = this;
    }

    #region 一般消息
    EventDispathcer<GameEvent> m_pEventDispathcer = new EventDispathcer<GameEvent>();
    public void RegistEvent(GameEvent EventID,RegistFunction pFunction)
	{	
		m_pEventDispathcer.RegistEvent(EventID,pFunction);
	}
	public void UnRegistEvent(GameEvent EventID,RegistFunction pFunction)
	{
		m_pEventDispathcer.UnRegistEvent(EventID,pFunction);
	}
	public object SendEvent(GameEvent EventID, params object[] pSender)
	{
        object Tmp = m_pEventDispathcer.HandleEvent(EventID, pSender);
		return Tmp;
	}
    #endregion
    #region 一般消息
    EventDispathcer<CommandName> m_pEventDispathcerCmd = new EventDispathcer<CommandName>();
    public void RegistCommand(CommandName EventID, RegistFunction pFunction)
    {
        m_pEventDispathcerCmd.RegistEvent(EventID, pFunction);
    }
    public void UnRegistCommand(CommandName EventID, RegistFunction pFunction)
    {
        m_pEventDispathcerCmd.UnRegistEvent(EventID, pFunction);
    }
    public object SendCommand(CommandName EventID, params object[] pSender)
    {
        object Tmp = m_pEventDispathcerCmd.HandleEvent(EventID, pSender);
        return Tmp;
    }
    #endregion
}
