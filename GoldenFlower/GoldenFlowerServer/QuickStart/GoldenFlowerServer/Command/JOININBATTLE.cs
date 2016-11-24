using SuperSocket.SocketBase.Protocol;
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

            //添加进战斗
            BattleController.Instance.OnHandleJoinBattle(playerId);

            UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
            SessionSendWithRecycle<defaultproto.RepJoinBattle>(session, rep_pool);

            //临时代码 够3个人，就是开始
            List<EntityGambler> entityGamblerList = BattleController.Instance.GetEntityGamblers();
            if (!entityGamblerList.IsNullOrEmpty() && entityGamblerList.Count >= 3)
                BattleController.Instance.RoundStart();
        }
    }
}
