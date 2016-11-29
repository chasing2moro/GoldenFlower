using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TipManager : MonoBehaviour {
    public Text m_TextTip;
    public GameObject m_CommonTip;
    public Text m_TextCommonTip;
	// Use this for initialization
	void Start () {
        m_TextTip.text = "";
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {

    }

    void OnEnable()
    {
        Facade.Instance.RegistEvent(GameEvent.UI_ShowTinyTip, OnHandleShowTip);
        Facade.Instance.RegistEvent(GameEvent.UI_ShowCommonTip, OnHandleShowTip);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistEvent(GameEvent.UI_ShowTinyTip, OnHandleShowTip);
        Facade.Instance.UnRegistEvent(GameEvent.UI_ShowCommonTip, OnHandleShowTip);
    }

    object OnHandleShowTip(params object[] vArgs)
    {
        if (IsInvoking("HideTextTip"))
        {
            CancelInvoke("HideTextTip");
        }
        m_TextTip.text = vArgs[0] as string;
        float closeTime = 2;
        if (vArgs.Length > 1)
            closeTime = (float)vArgs[1];
        Invoke("HideTextTip", closeTime);
        return null;
    }

    void HideTextTip()
    {
        m_TextTip.text = "";
    }

    System.Action<TipButtonType> _buttonClickedCallBack;
    object OnHandleShowCommonTip(params object[] vArgs)
    {
        m_CommonTip.SetActive(true);
        m_TextCommonTip.text = vArgs[0] as string;
        _buttonClickedCallBack = vArgs[1] as System.Action<TipButtonType>;
        return null;
    }

    public void OnButtonClicked(int vIndex)
    {
        if (_buttonClickedCallBack != null)
            _buttonClickedCallBack((TipButtonType)vIndex);

        m_CommonTip.SetActive(false);
    }
}

public enum TipButtonType
{
    Cancel,
    Ok,
}
