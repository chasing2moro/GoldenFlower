using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleHandle : UnityEngine.MonoBehaviour
{
    //临时代码
    public static BattleHandle Instance;
    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }
     void OnEnable()
    {

    }

     void OnDisable()
    {

    }
}

