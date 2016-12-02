using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StateThink : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        if (DataManagerPlayer.Instance.IsMySelf(_target.GetPlayerId()))
        {
            Logger.Log("StateThink");
        }
        else
        {
            Logger.Log("StateThink:" + _target.GetPlayerId());
        }
        Facade.Instance.SendEvent(GameEvent.Battle_EnterStateThink, _target.GetPlayerId());
#endif

    }
}

