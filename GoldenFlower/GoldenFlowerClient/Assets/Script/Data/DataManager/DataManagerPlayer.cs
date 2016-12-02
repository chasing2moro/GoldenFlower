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

    public int money = 0;
    public int coin = 0;

    public bool IsMySelf(int vPlayerId) {
        return m_PlayerId == vPlayerId;
    }

    public void OnHandleUpdateResource(defaultproto.UpdateResource vProto)
    {
        foreach (defaultproto.Resource resource in vProto.resource)
        {
            switch (resource.type)
            {
                case defaultproto.ResourceType.Money:
                    money += resource.count;
                    break;
                case defaultproto.ResourceType.Coin:
                    coin += resource.count;
                    break;
                default:
                    break;
            }
        }
    }

}

