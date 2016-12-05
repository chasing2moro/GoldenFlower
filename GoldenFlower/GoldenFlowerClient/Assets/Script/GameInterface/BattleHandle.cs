using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BattleHandle : UnityEngine.MonoBehaviour
{
    public static BattleHandle Instance;
    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }
     void OnEnable()
    {
        Facade.Instance.RegistCommand(CommandName.JOININBATTLE, OnHandleJoinInBattle);
        Facade.Instance.RegistCommand(CommandName.UPDATEJOININBATTLEFINISH, OnHandleJoinInBattleFinish);
        Facade.Instance.RegistCommand(CommandName.UPDATEDEALCARD, OnHandleUpdateDealCard);
        Facade.Instance.RegistCommand(CommandName.UPDATEDEALCARDFISH, OnHandleUpdateDealCardFinish);
        Facade.Instance.RegistCommand(CommandName.BET, OnHandleBet);
        Facade.Instance.RegistCommand(CommandName.QUIT, OnHandleQuit);
        Facade.Instance.RegistCommand(CommandName.UPDATEROUNDFINISH, OnHandleUpdateRoundFinish);
    }

     void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.JOININBATTLE, OnHandleJoinInBattle);
        Facade.Instance.UnRegistCommand(CommandName.UPDATEJOININBATTLEFINISH, OnHandleJoinInBattleFinish);
        Facade.Instance.UnRegistCommand(CommandName.UPDATEDEALCARD, OnHandleUpdateDealCard);
        Facade.Instance.UnRegistCommand(CommandName.UPDATEDEALCARDFISH, OnHandleUpdateDealCardFinish);
        Facade.Instance.UnRegistCommand(CommandName.BET, OnHandleBet);
        Facade.Instance.UnRegistCommand(CommandName.QUIT, OnHandleQuit);
        Facade.Instance.UnRegistCommand(CommandName.UPDATEROUNDFINISH, OnHandleUpdateRoundFinish);
    }

    object OnHandleJoinInBattle(params object[] args)
    {
        defaultproto.RepJoinBattle repJoinBattle = args[0] as defaultproto.RepJoinBattle;

        BattleController.Instance.OnHandleJoinBattle(repJoinBattle.playerId, repJoinBattle.index);
        return null;
    }

    object OnHandleJoinInBattleFinish(params object[] args)
    {
        BattleController.Instance.OnHandleJoinBattleFinish();
        return null;
    }

    object OnHandleUpdateDealCard(params object[] args)
    {
        defaultproto.UpdateDealCard updateDealCard = args[0] as defaultproto.UpdateDealCard;
        List<CardData> cardList = new List<CardData>();
        for (int i = 0; i < updateDealCard.cardDatas.Count; i++)
        {
            CardData cardData = new CardData();
            cardData.m_CardType = (CardType)updateDealCard.cardDatas[i].cardType;
            cardData.m_Rank = updateDealCard.cardDatas[i].rank;
            cardList.Add(cardData);
        }
        BattleController.Instance.OnHandleDealCard(cardList);
        return null;
    }
    object OnHandleUpdateDealCardFinish(params object[] args)
    {
        BattleController.Instance.OnHandleDealCardFinish();
        return null;
    }

  
     object OnHandleBet(params object[] args)
    {
        defaultproto.RepBet repBet = args[0] as defaultproto.RepBet;
        if(repBet.result.errorCode == defaultproto.ErrorCode.None)
        {
            BattleController.Instance.OnHandlePlayerBet(repBet.playerId, repBet.count);
        }
        else
        {
            Logger.LogError("有错误，不能继续流程，请排除黑客操作");
        }

        return null;
    }

    object OnHandleQuit(params object[] args)
    {
        defaultproto.RepQuit repBet = args[0] as defaultproto.RepQuit;
        if (repBet.result.errorCode == defaultproto.ErrorCode.None)
        {
            BattleController.Instance.OnHandlePlayerQuit(repBet.playerId);
        }
        else
        {
            Logger.LogError("有错误，不能继续流程，请排除黑客操作");
        }

        return null;
    }

    object OnHandleUpdateRoundFinish(params object[] args)
    {
        return null;
    }
}

