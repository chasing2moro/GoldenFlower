using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 下注
/// </summary>
public class StateBet : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        if (DataManagerPlayer.Instance.IsMySelf(_target.GetPlayerId()))
        {
            Logger.Log("StateBet");
        }
        else
        {
            Logger.Log("StateBet:" + _target.GetPlayerId());
        }
#endif
        //投注完马上跳下一个状态
        GetTarget<EntityGambler>().Idle();
    }
}

