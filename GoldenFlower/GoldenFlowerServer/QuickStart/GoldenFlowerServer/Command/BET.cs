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
   

            //sessiong找不到玩家
            if (playerId < 0)
            {
                defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "Session对应的玩家id找不到");
                SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
                return;
            }

            //处理非法炒作（不是轮到他，他就下注的）
            string vErrorDes;
            defaultproto.ErrorCode errorCode = BattleController.Instance.IsValidBetOperation(playerId, out vErrorDes);
            if (errorCode != defaultproto.ErrorCode.None)
            {
                defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();
                UtilityMsgHandle.AssignErrorDes(rep_pool, errorCode, vErrorDes);
                SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
                return;
            }

            //处理并返回处理了的玩家
            EntityGambler entityGambler = BattleController.Instance.OnHandlePlayerBet(playerId, req.count);
            if(entityGambler == null)
            {
                defaultproto.RepBet rep_pool = UtilityObjectPool.Instance.Dequeue<defaultproto.RepBet>();
                UtilityMsgHandle.AssignErrorDes(rep_pool, defaultproto.ErrorCode.InternalError, "战场找不到玩家，id:" + playerId);
                SessionSendWithRecycle<defaultproto.RepBet>(session, rep_pool);
                return;
            }
        }
    }
}
