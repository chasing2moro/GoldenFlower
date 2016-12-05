using System.Collections;

public enum GameEvent
{
    Socket_Connected,
    Socket_Disconnected,

    Battle_EnterStateBet,
    Battle_EnterStateIdle,
    Battle_EnterStateQuit,
    Battle_EnterStateThink,
    Battle_EnterStateLose,

    UI_ShowTinyTip,
    UI_HitTinyTip,
    UI_ShowCommonTip,
    UI_HitCommonTip,
    Login
}
