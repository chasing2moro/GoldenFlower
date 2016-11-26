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
        Facade.Instance.RegistCommand(CommandName.BET, OnHandleBet);

    }

     void OnDisable()
    {
        Facade.Instance.UnRegistCommand(CommandName.JOININBATTLE, OnHandleJoinInBattle);
        Facade.Instance.UnRegistCommand(CommandName.UPDATEJOININBATTLEFINISH, OnHandleJoinInBattleFinish);
        Facade.Instance.UnRegistCommand(CommandName.BET, OnHandleBet);
    }

    object OnHandleJoinInBattle(params object[] args)
    {
        defaultproto.RepJoinBattle repJoinBattle = new defaultproto.RepJoinBattle();

        BattleQueue.Instance.AddGambler(repJoinBattle.playerId, repJoinBattle.index);
        BattleController.Instance.OnHandleJoinBattle(repJoinBattle.playerId);
        return null;
    }

    object OnHandleJoinInBattleFinish(params object[] args)
    {
        BattleController.Instance.OnHandleJoinBattleFinish();
        return null;
    }

     object OnHandleBet(params object[] args)
    {
        defaultproto.RepBet repBet = new defaultproto.RepBet();
        BattleController.Instance.OnHandlePlayerBet(repBet.playerId, repBet.count);
        return null;
    }
}

