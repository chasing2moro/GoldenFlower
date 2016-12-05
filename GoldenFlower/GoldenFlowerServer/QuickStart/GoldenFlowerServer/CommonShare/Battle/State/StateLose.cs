using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// State bet
/// </summary>
public class StateLose : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        if (DataManagerPlayer.Instance.IsMySelf(_target.GetPlayerId()))
        {
            Logger.Log("StateLose");
        }
        else
        {
            Logger.Log("StateLose:" + _target.GetPlayerId());
        }
        Facade.Instance.SendEvent(GameEvent.Battle_EnterStateLose, _target.GetPlayerId());
#endif
    }

}

