using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleController 
{
    //临时代码
    public static BattleController Instance = new BattleController();

    SortedDictionary<int, EntityGambler> _id2EntityGambler = new SortedDictionary<int, EntityGambler>();
    SortedDictionary<int, EntityGambler>.Enumerator _id2EntityGamblerEnumerator;

    EntityGambler _curGambler;
    EntityGambler _previousGambler;
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


    public int EntityGamblerCount
    {
        get
        {
            return _id2EntityGambler.Count;
        }
    }

    public SortedDictionary<int, EntityGambler> Id2EntityGambler
    {
        get
        {
            return _id2EntityGambler;
        }
    }

    public defaultproto.ErrorCode IsValidBetOperation(int vPlyaerId, out string vDes)
    {
        return IsValidOperation(vPlyaerId, out vDes);
    }
    public defaultproto.ErrorCode IsValidQuitOperation(int vPlyaerId, out string vDes)
    {
        return IsValidOperation(vPlyaerId, out vDes);
    }
    public defaultproto.ErrorCode IsValidOperation(int vPlyaerId, out string vDes)
    {
        if (_curGambler == null)
        {
            vDes = "游戏还没开始";
            return defaultproto.ErrorCode.InvalidOperation;
        }

        if (_curGambler.GetPlayerId() != vPlyaerId)
        {
            vDes = "还没轮到你，你就操作了,轮到:" + _curGambler.GetPlayerId();
            return defaultproto.ErrorCode.InvalidOperation;
        }

        vDes = null;
        return defaultproto.ErrorCode.None;
    }

    //int __num;
    ///// <summary>
    ///// 获取生存人数
    ///// </summary>
    ///// <returns>返回所有生存的人数， 包含vPlayerId。 如果生存人数不等于1，则vPlayerId无效</returns>
    //public int GetAliveNum(out int vPlayerId)
    //{
    //    __num = 0;
    //    vPlayerId = 0;
    //    foreach (EntityGambler entityGambler in _id2EntityGambler.Values)
    //    {
    //        if (entityGambler.IsAlive())
    //        {
    //            ++__num;
    //            vPlayerId = entityGambler.GetPlayerId();
    //        }
    //    }

    //    return __num;
    //}

    //重置所有玩家状态
    void ClearAllEntityGamblerState()
    {
        foreach (EntityGambler entityGambler in Id2EntityGambler.Values)
        {
            entityGambler.ClearState();
            entityGambler.m_Next = null;
        }
    }

    /// <summary>
    /// Rund start and send card
    /// </summary>
    void RoundStart()
    {
        //重置玩家状态
        ClearAllEntityGamblerState();

        //获取枚举器,游标自动移到开头
        _id2EntityGamblerEnumerator = _id2EntityGambler.GetEnumerator();

        _previousGambler = null;
        _curGambler = null;

        //这里形成链
        foreach (EntityGambler entityGambler in _id2EntityGambler.Values)
        {
            if (_curGambler != null)
            {
                _curGambler.m_Next = entityGambler;
            }
            _previousGambler = _curGambler;
            _curGambler = entityGambler;
        }
        //在这里形成闭环
        _curGambler.m_Next = _id2EntityGambler.First().Value;


        //send card to user
        CardBox.ReqDealCard(EntityGamblerCount, 3, this);
    }

    /// <summary>
    /// 轮到下一个人
    /// </summary>
    void TurnNext()
    {
        //如果最后只有一个人存活，则比赛结束
        if (_curGambler.m_Next == _curGambler)
        {
            //游戏结束
            RoundFinish(_curGambler.GetPlayerId());
            return;
        }

        //保存上一个玩家
        if(_curGambler.IsAlive())
            _previousGambler = _curGambler;

        //轮到这个玩家
        _curGambler = _curGambler.m_Next;

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

        //进入下一个状态
        entityGambler.Bet(vCount);


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

        //轮到下一位
        TurnNext();

        return entityGambler;
    }

    public EntityGambler OnHandlePlayerQuit(int vPalyerId)
    {
        EntityGambler entityGambler = GetEntityGambler(vPalyerId);
        if (entityGambler == null)
        {
            Logger.LogError("找不到：" + vPalyerId + " 这个玩家");
            return null;
        }

        if(_curGambler != entityGambler)
        {
            Logger.LogError("还没轮到" + entityGambler.GetPlayerId());
        }

        //Quit状态
        entityGambler.Quit();
        //把自己从列表移除
        _previousGambler.m_Next = _curGambler.m_Next;

#if !UNITY_CLIENT
        defaultproto.RepQuit rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepQuit>();
        rep_pool.playerId = entityGambler.GetPlayerId();
        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.QUIT,
            rep_pool,
            _id2EntityGambler.Values.ToArray());
        UtilityObjectPool.Instance.Enqueue<defaultproto.RepQuit>(rep_pool);
#endif


        //轮到下一位
        TurnNext();
        return entityGambler;
    }
    /// <summary>
    /// Handle send card to user
    /// </summary>
    public EntityGambler OnHandleDealCard(List<CardData> vCardList)
    {
#if UNITY_CLIENT
        EntityGambler entityGambler = GetEntityGambler(DataManagerPlayer.Instance.m_PlayerId);
        entityGambler.SetCardList(vCardList);
        return entityGambler;
#else
        _previousGambler = _curGambler;
        _curGambler.m_Next.SetCardList(vCardList);
        _curGambler = _curGambler.m_Next;

        return _curGambler;
#endif
    }

    /// <summary>
    /// Handle send card to user all finish
    /// </summary>
    public void OnHandleDealCardFinish()
    {
    
        // turn to the first one
        TurnNext();
    }

    public void RoundFinish(int vWinPlayer)
    {
        Logger.Log("玩家：" + vWinPlayer + " 胜利了, 重新开始");

//#if !UNITY_CLIENT
//        defaultproto.UpdateRoundFinish rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.UpdateRoundFinish>();
//        rep_pool.winerPlayerId = vWinPlayer;
//        UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
//        UtilityMsgHandle.BrocastMsgWithEntityGamblers(CommandName.UPDATEROUNDFINISH,
//            rep_pool,
//            _id2EntityGambler.Values.ToArray());
//        UtilityObjectPool.Instance.Enqueue<defaultproto.UpdateRoundFinish>(rep_pool);
//#endif

        RoundStart();
    }
}

