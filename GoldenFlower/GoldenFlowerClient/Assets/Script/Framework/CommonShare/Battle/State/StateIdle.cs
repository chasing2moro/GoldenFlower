using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StateIdle : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        if (DataManagerPlayer.Instance.IsMySelf(_target.GetPlayerId()))
        {
            Logger.Log("StateIdle");
        }
        else
        {
            Logger.Log("StateIdle:" + _target.GetPlayerId());
        }
#endif
    }
}

