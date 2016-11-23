﻿using System;
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
     int _playerId;
    //card in hand
    public List<CardData> m_CardList = new List<CardData>();
    //session for player communicate
    public CustomProtocolSession m_Session;

    public void AddCard(CardData vCardData)
    {
        m_CardList.Add(vCardData);
    }
    public void EmptyCard()
    {
        m_CardList.Clear();
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

    public int GetPlayerId()
    {
        return _playerId;
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
