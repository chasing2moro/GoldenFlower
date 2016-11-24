using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public Transform m_UIRoot;
    public GameObject[] m_UIList;
   
    void Awake()
    {
        Instance = this;
    }

    public UIBase ShowUI(UIType vUIType, params object[] vArgs)
    {
        UIBase ui = null;
        foreach (GameObject uiBase in m_UIList)
        {
            if(uiBase.name == vUIType.ToString())
            {
                GameObject obj = Instantiate(uiBase);
                ui = obj.GetComponent<UIBase>();
            }
        }
        if (ui == null)
        {
            Debug.LogError("找不到UI：" + vUIType.ToString());
        }
        else
        {
            ui.gameObject.name = vUIType.ToString();
            ui.transform.SetParent(m_UIRoot, false);
            ui.OnShow(vArgs);
        }

        return ui;
    }

    public void HideUI(UIType vUIType)
    {
        UIBase[] uiBases = m_UIRoot.GetComponentsInChildren<UIBase>();

        UIBase ui = null;
        foreach (UIBase uiBase in uiBases)
        {
            if(uiBase.gameObject.name == vUIType.ToString())
            {
                ui = uiBase;
            }
        }

        if (ui == null)
        {
            Debug.LogError("关闭时 找不到UI：" + vUIType.ToString());
        }
        else
        {
            GameObject.Destroy(ui.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
