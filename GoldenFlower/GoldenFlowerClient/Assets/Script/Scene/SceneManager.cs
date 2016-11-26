using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{

    public static SceneManager Instance;
    public GameObject[] m_ScenePrefabCaches;
    void Awake()
    {
        Instance = this;
    }

    public void LoadScene(SceneType vSceneType)
    {
        //隐藏UI
        UIManager.Instance.HideAll();

        //销毁旧的场景
        InterfaceBase[] childs = GetComponentsInChildren<InterfaceBase>();
        foreach (var child in childs)
        {
            Destroy(child.gameObject);
        }

        //打开新的场景
        foreach (GameObject scenePrefabCache in m_ScenePrefabCaches)
        {
            if (scenePrefabCache.name == vSceneType.ToString())
            {
                GameObject scene = Instantiate(scenePrefabCache);
                scene.transform.SetParent(this.transform, false);
            }
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum SceneType
{
    Main,
    Battle,
}