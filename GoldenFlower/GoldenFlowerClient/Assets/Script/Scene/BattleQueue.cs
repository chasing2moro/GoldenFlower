using System;
using System.Collections.Generic;

public class BattleQueue
{
    public static BattleQueue Instance = new BattleQueue();

    public Dictionary<int, int> m_Index2PlayerId = new Dictionary<int, int>();
    /// <summary>
    /// 
    /// </summary>
    public void AddGambler(int vPlayerId, int vIndex)
    {
        m_Index2PlayerId[vIndex] = vPlayerId;
    }
}

