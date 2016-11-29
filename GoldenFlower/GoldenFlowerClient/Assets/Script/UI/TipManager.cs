using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TipManager : MonoBehaviour {
    public Text m_TextTip;
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
        Facade.Instance.RegistEvent(GameEvent.UI_ShowTip, OnHandleShowTip);
    }

    void OnDisable()
    {
        Facade.Instance.UnRegistEvent(GameEvent.UI_ShowTip, OnHandleShowTip);
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
}
