using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// gambler 
//英['gæmblə(r)]  
//n.赌徒；
/// </summary>
public class EntityGambler : EntityBase
{

    //card in hand
     List<CardData> _cardList = new List<CardData>();
    ////session for player communicate
    //public CustomProtocolSession m_Session;
    public int m_Index;
    public void SetCardList(List<CardData> vCardDataList)
    {
        _cardList = vCardDataList;
    }

    /// <summary>
    /// 初始化入口
    /// </summary>
    /// <param name="vPlayerId"></param>
    public void SetPlayerId(int vPlayerId)
    {
        //玩家id
        _playerId = vPlayerId;

        //状态机初始化
        _stateBet.SetTarget(this);
        _stateIdle.SetTarget(this);
        _stateQuit.SetTarget(this);
        _stateThink.SetTarget(this);
    }

    StateBet _stateBet = new StateBet();
    StateIdle _stateIdle = new StateIdle();
    StateQuit _stateQuit = new StateQuit();
    StateThink _stateThink = new StateThink();

    //思考
    public void Think()
    {
        _stateThink.OnEnterState();
    }

    //下注
    public void Bet()
    {
        _stateBet.OnEnterState();
    }

    //放弃
    public void Quit()
    {
        _stateQuit.OnEnterState();
    }

    //空闲
    public void Idle()
    {
        _stateIdle.OnEnterState();
    }
}

