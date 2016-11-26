using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StateThink : StateBase
{
    public override void OnEnterState()
    {
#if UNITY_CLIENT
        defaultproto.ReqBet reqBet = new defaultproto.ReqBet();
        reqBet.count = 20;
        SocketManager.Instance.SendMsg(CommandName.BET, reqBet);
#endif
        //client 玩家需要请求下注
        //socket 等待玩家下注网络请求
    }
}

