using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// State bet
/// </summary>
public class StateBet : StateBase
{
    public override void OnEnterState()
    {
        //jump to idle state immediately
        GetTarget<EntityGambler>().Idle();
    }

    public void OnEnterState(int vBetCount)
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
        Facade.Instance.SendEvent(GameEvent.Battle_EnterStateBet, _target.GetPlayerId(), vBetCount);
#endif
        OnEnterState();


    }
}

