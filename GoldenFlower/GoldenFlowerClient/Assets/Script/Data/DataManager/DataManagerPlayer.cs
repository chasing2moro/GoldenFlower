using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerPlayer : DataManagerBase
{
    public static DataManagerPlayer Instance;
    void Awake()
    {
        Instance = this;
    }

    public int m_PlayerId;
    public string m_UserName;

}

