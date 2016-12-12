using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public Transform m_UIRoot;
    public Transform m_TipRoot;
    public GameObject[] m_UIList;
    public GameObject m_Tip;
   
    void Awake()
    {
        Instance = this;
    }

    public void ShowTip()
    {
        GameObject obj = Instantiate(m_Tip);
        obj.transform.SetParent(m_TipRoot, false);
    }

    public T ShowUI<T>(params object[] vArgs)  where T: UIBase
    {
        T ui = null;
        string uiName = typeof(T).ToString();
        foreach (GameObject uiBase in m_UIList)
        {
            if(uiBase.name == uiName)
            {
                GameObject obj = Instantiate(uiBase);
                ui = obj.GetComponent<T>();
            }
        }
        if (ui == null)
        {
            Debug.LogError("找不到UI：" + uiName);
        }
        else
        {
            ui.gameObject.name = uiName;
            ui.transform.SetParent(m_UIRoot, false);
            ui.OnShow(vArgs);
        }

        return ui;
    }

    public void HideUI<T>()
    {
        UIBase[] uiBases = m_UIRoot.GetComponentsInChildren<UIBase>();

        UIBase ui = null;
        string uiName = typeof(T).ToString();
        foreach (UIBase uiBase in uiBases)
        {
            if(uiBase.gameObject.name == uiName)
            {
                ui = uiBase;
                break;
            }
        }

        if (ui == null)
        {
            Debug.LogError("关闭时 找不到UI：" + uiName);
        }
        else
        {
            GameObject.Destroy(ui.gameObject);
        }
    }

    public void HideAll()
    {
        UIBase[] uiBases = m_UIRoot.GetComponentsInChildren<UIBase>();
        foreach (UIBase uiBase in uiBases)
        {
            GameObject.Destroy(uiBase.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
