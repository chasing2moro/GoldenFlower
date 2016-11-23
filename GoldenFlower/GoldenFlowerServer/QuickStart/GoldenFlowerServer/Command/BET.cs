using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace SuperSocket.QuickStart.CustomProtocol.Command
{
    public class BET : MyCommandBase
    {
        public override void ExecuteCommand(CustomProtocolSession session, BinaryRequestInfo requestInfo)
        {
            int playerId = PlayerDataManager.Instance.GetPlayerId(session);
            defaultproto.ReqBet req = UtilityProbuff.DeSerialize<defaultproto.ReqBet>(requestInfo.Body);
            defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();

            //sessiong找不到玩家
            if (playerId < 0)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "Session对应的玩家id找不到");
                SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
                return;
            }

            //处理并返回处理了的玩家
            EntityGambler entityGambler = BattleController.Instance.OnHandlePlayerBet(playerId);
            if(entityGambler == null)
            {
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "战场找不到玩家，id:" + playerId);
                SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
                return;
            }

#warning 扣除玩家的钱代码没有写
            UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.None);
            SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
        }
    }
}
