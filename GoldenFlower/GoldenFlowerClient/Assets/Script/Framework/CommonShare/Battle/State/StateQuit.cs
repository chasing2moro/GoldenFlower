using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 不跟
/// </summary>
public class StateQuit : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        if (DataManagerPlayer.Instance.IsMySelf(_target.GetPlayerId()))
        {
            Logger.Log("StateQuit");
        }
        else
        {
            Logger.Log("StateQuit:" + _target.GetPlayerId());
        }
#endif
    }
}

