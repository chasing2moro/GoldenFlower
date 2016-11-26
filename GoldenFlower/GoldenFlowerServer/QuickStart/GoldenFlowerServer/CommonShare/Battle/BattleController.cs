using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleController 
{
    //临时代码
    public static BattleController Instance = new BattleController();

    Dictionary<int, EntityGambler> _id2EntityGambler = new Dictionary<int, EntityGambler>();

    int _curIndex = 0;
    EntityGambler _curGambler;
    EntityGambler AddPlayer(int vPlayerId, int vIndex)
    {
        EntityGambler gambler_pool = UtilityObjectPool.Instance.Dequeue<EntityGambler>();
        //玩家id
        gambler_pool.SetPlayerId(vPlayerId);
        //位置
        if (vIndex == -1)
            gambler_pool.m_Index = _id2EntityGambler.Count;
        else
            gambler_pool.m_Index = vIndex;

        _id2EntityGambler[vPlayerId] = gambler_pool;

        return gambler_pool;
    }

    EntityGambler GetEntityGambler(int vPlayerId)
    {
        EntityGambler entityGambler = null;
       if( !_id2EntityGambler.TryGetValue(vPlayerId, out entityGambler))
        {
            Logger.LogError("can not find player");
        }
        return entityGambler;
    }

    List<EntityGambler> _entityGamblerList;
    public List<EntityGambler> EntityGamblerList
    {
        get
        {
            return  _entityGamblerList;
        }
    }

    public int EntityGamblerCount
    {
        get
        {
            return _id2EntityGambler.Count;
        }
    }

    public Dictionary<int, EntityGambler> Id2EntityGambler
    {
        get
        {
            return _id2EntityGambler;
        }
    }
    /// <summary>
    /// Rund start and send card
    /// </summary>
    void RoundStart()
    {
        //构建List
        _entityGamblerList = new List<EntityGambler>(_id2EntityGambler.Values);
        _entityGamblerList.Sort((x, y) => x.m_Index - y.m_Index);

        CardBox.ReqDealCard(EntityGamblerCount, 3, this);
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
        _curIndex = ++_curIndex % EntityGamblerCount;

        //轮到这个赌徒
        _curGambler = EntityGamblerList[_curIndex];

        //思考再下注
        _curGambler.Think();
    }

    /// <summary>
    /// 处理玩家加入
    /// </summary>
    /// <param name="vPlayerId"></param>
    public void OnHandleJoinBattle(int vPlayerId, int vIndex = -1)
    {
        EntityGambler entityGambler = AddPlayer(vPlayerId, vIndex);

        //广播给其他人
#if !UNITY_CLIENT
        defaultproto.RepJoinBattle rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepJoinBattle>();
        rep_pool.playerId = vPlayerId;
        rep_pool.index = entityGambler.m_Index;
        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None, "有人加入，广播给其他人");
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.JOININBATTLE, 
            rep_pool,
            _id2EntityGambler.Values.ToArray());
#endif
    }

    /// <summary>
    /// 处理玩家加入
    /// </summary>
    /// <param name="vPlayerId"></param>
    public void OnHandleJoinBattleFinish()
    {
        //广播给其他人
#if !UNITY_CLIENT
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.UPDATEJOININBATTLEFINISH,
            null, _id2EntityGambler.Values.ToArray());
#endif

        //告诉人家再发牌，此处要粘包发出去，要不然先后顺序出bug
        RoundStart();
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
        {
            Logger.LogError("找不到：" + vPalyerId + " 这个玩家");
            return null;
        }

#if !UNITY_CLIENT
        //#warning 扣除玩家的钱代码没有写
        defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();
        rep_pool.count = vCount;
        rep_pool.playerId = entityGambler.GetPlayerId();
        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.BET,
            rep_pool,
            _id2EntityGambler.Values.ToArray());
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
        EntityGambler entityGambler = EntityGamblerList[vPlayerIndex];
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

