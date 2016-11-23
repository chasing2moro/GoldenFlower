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
        //投注完马上跳下一个状态
        GetTarget<EntityGambler>().Idle();
    }
}

