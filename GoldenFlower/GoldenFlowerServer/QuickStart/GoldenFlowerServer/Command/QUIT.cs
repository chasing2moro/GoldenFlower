using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class QUIT : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            int playerId = PlayerDataManager.Instance.GetPlayerId(session);

            defaultproto.RepQuit rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepQuit>();

            //sessiong找不到玩家
            if (playerId < 0)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "Session对应的玩家id找不到");
                SessionSendWithRecycle<defaultproto.RepQuit>(session, rep_pool);
                return;
            }

            //处理非法操作（不是轮到他，他就放弃的）
            string vErrorDes;
            defaultproto.ErrorCode errorCode = BattleController.Instance.IsValidQuitOperation(playerId, out vErrorDes);
            if (errorCode != defaultproto.ErrorCode.None)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, errorCode, vErrorDes);
                SessionSendWithRecycle<defaultproto.RepQuit>(session, rep_pool);
                return;
            }

            //处理并返回处理了的玩家
            EntityGambler entityGambler = BattleController.Instance.OnHandlePlayerQuit(playerId);
            if (entityGambler == null)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "战场找不到玩家，id:" + playerId);
                SessionSendWithRecycle<defaultproto.RepQuit>(session, rep_pool);
                return;
            }
        }
    }
}
