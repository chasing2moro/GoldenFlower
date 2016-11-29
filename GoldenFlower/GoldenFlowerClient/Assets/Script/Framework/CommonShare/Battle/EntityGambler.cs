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
    public FSMState m_State;
    //card in hand
     List<CardData> _cardList = new List<CardData>();

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
        m_State = FSMState.Think;
        _stateThink.OnEnterState();
    }

    //下注
    public void Bet()
    {
        m_State = FSMState.Bet;
        _stateBet.OnEnterState();
    }

    //放弃
    public void Quit()
    {
        m_State = FSMState.Quit;
        _stateQuit.OnEnterState();
    }

    //空闲
    public void Idle()
    {
        m_State = FSMState.Idle;
        _stateIdle.OnEnterState();
    }

    public void ClearState()
    {
        Idle();
    }
}

