﻿using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class JOININBATTLE : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            int playerId = PlayerDataManager.Instance.GetPlayerId(session);
            defaultproto.RepJoinBattle rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepJoinBattle>();

            //找不到玩家
            if (playerId < 0)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "Session对应的玩家id找不到");
                SessionSendWithRecycle<defaultproto.RepJoinBattle>(session, rep_pool);
                return;
            }

            //自己进之前，已经有人了，所以也要告诉玩家
            List<EntityGambler> entityGamblers = BattleController.Instance.GetEntityGamblers();
            for (int i = 0; i < entityGamblers.Count; i++)
            {
                rep_pool.playerId = entityGamblers[i].GetPlayerId();
                rep_pool.index = i;
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None, "其他玩家已经加入");
                SessionSend(session, rep_pool);
            }
            UtilityObjectPool.Instance.Enqueue<defaultproto.RepJoinBattle>(rep_pool);

            //添加进战斗,并广播给其他人
            BattleController.Instance.OnHandleJoinBattle(playerId);

            //临时代码 够3个人，就是开始
            List<EntityGambler> entityGamblerList = BattleController.Instance.GetEntityGamblers();
            if (!entityGamblerList.IsNullOrEmpty() && entityGamblerList.Count >= 3)
                BattleController.Instance.RoundStart();
        }
    }
}
