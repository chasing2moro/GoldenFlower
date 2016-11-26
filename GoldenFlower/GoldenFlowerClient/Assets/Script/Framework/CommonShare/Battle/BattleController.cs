using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleController 
{
    //临时代码
    public static BattleController Instance = new BattleController();

    List<EntityGambler> _entityGamblerList = new List<EntityGambler>();
    int _curIndex = 0;
    EntityGambler _curGambler;
     void AddPlayer(int vPlayerId)
    {
        EntityGambler gambler_pool = UtilityObjectPool.Instance.Dequeue<EntityGambler>();
        //玩家id
        gambler_pool.SetPlayerId(vPlayerId);

        _entityGamblerList.Add(gambler_pool);
    }

    EntityGambler GetEntityGambler(int vPlayerId)
    {
        EntityGambler entityGambler = null;
        foreach (var item in _entityGamblerList)
        {
            if(item.GetPlayerId() == vPlayerId)
            {
                entityGambler = item;
            }
        }
        return entityGambler;
    }

    public List<EntityGambler> GetEntityGamblers()
    {
        return _entityGamblerList;
    }

    /// <summary>
    /// Rund start and send card
    /// </summary>
    public void RoundStart()
    {
        CardBox.ReqDealCard(_entityGamblerList.Count, 3, this);
    }


    // void Stop()
    //{
    //    //第二轮重新开始
    //    RoundStart();
    //}

    /// <summary>
    /// 轮到下一个人
    /// </summary>
    void TurnNext()
    {
        _curIndex = ++_curIndex % _entityGamblerList.Count;

        //轮到这个赌徒
        _curGambler = _entityGamblerList[_curIndex];

        //思考再下注
        _curGambler.Think();
    }

    /// <summary>
    /// 处理玩家加入
    /// </summary>
    /// <param name="vPlayerId"></param>
    public void OnHandleJoinBattle(int vPlayerId)
    {
        AddPlayer(vPlayerId);

        //广播给其他人
#if !UNITY_CLIENT
        defaultproto.RepJoinBattle rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepJoinBattle>();
        rep_pool.playerId = vPlayerId;
        rep_pool.index = _entityGamblerList.Count - 1;
        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None, "有人加入，广播给其他人");
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.JOININBATTLE, rep_pool, _entityGamblerList.ToArray());
#endif
    }

    /// <summary>
    /// 处理玩家加入
    /// </summary>
    /// <param name="vPlayerId"></param>
    public void OnHandleJoinBattleFinish()
    {
        RoundStart();

        //广播给其他人
#if !UNITY_CLIENT
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.UPDATEJOININBATTLEFINISH,
            null, _entityGamblerList.ToArray());
#endif
    }

    /// <summary>
    /// 处理玩家下注， 并返回处理了的玩家
    /// </summary>
    /// <param name="vPalyerId"></param>
    /// <returns></returns>
    public EntityGambler OnHandlePlayerBet(int vPalyerId, int vCount)
    {
        //战场里找玩家
        EntityGambler entityGambler = GetEntityGambler(vPalyerId);
        if (entityGambler == null)
            return null;

#if !UNITY_CLIENT
        #warning 扣除玩家的钱代码没有写
        defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();
        rep_pool.count = vCount;
        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.BET, rep_pool, _entityGamblerList.ToArray());
        UtilityObjectPool.Instance.Enqueue<defaultproto.RepBet>(rep_pool);
#endif
        //进入下一个状态
        entityGambler.Bet();

        //轮到下一位
        TurnNext();

        return entityGambler;
    }

    /// <summary>
    /// Handle send card to user
    /// </summary>
    public EntityGambler OnHandleDealCard(List<CardData> vCardList, int vPlayerIndex)
    {
        EntityGambler entityGambler = _entityGamblerList[vPlayerIndex];
        entityGambler.SetCardList(vCardList);
        return entityGambler;
    }

    /// <summary>
    /// Handle send card to user all finish
    /// </summary>
    public void OnHandleDealCardFinish()
    {
        _curIndex = -1;
        // turn to the first one
        TurnNext();
    }
}

