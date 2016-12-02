using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayerDataManager
{
    public static PlayerDataManager Instance;
    public static void CreateInstance()
    {
        Instance = new PlayerDataManager();
    }

    Dictionary<CustomProtocolSession, int> _session2PlayerId = new Dictionary<CustomProtocolSession, int>();
    Dictionary<int, CustomProtocolSession> _playerId2Session = new Dictionary<int, CustomProtocolSession>();
    Dictionary<int, PlayerInfo> _playerId2PlayerInfo = new Dictionary<int, PlayerInfo>();
    public void AddPlayer(CustomProtocolSession vSession, int vPlayerId)
    {
        _session2PlayerId[vSession] = vPlayerId;
        _playerId2Session[vPlayerId] = vSession;
    }

    public PlayerInfo GetPlayerInfo(int vPlayerId)
    {
        PlayerInfo playerInfo;
        if (!_playerId2PlayerInfo.TryGetValue(vPlayerId, out playerInfo))
        {
            playerInfo = new PlayerInfo();
            _playerId2PlayerInfo[vPlayerId] = playerInfo;
        }
        return playerInfo;
    }

    public void RemovePlayer(CustomProtocolSession vSession)
    {
        if (_session2PlayerId.ContainsKey(vSession))
        {
            int playerId = _session2PlayerId[vSession];

            _playerId2Session.Remove(playerId);
            _playerId2PlayerInfo.Remove(playerId);
            _session2PlayerId.Remove(vSession);

            Logger.LogError("remove vSession id: " + vSession.SessionID + " error");
        }
    }

    int __playerId;
    public int GetPlayerId(CustomProtocolSession vSession)
    {
        __playerId = -1;
        _session2PlayerId.TryGetValue(vSession, out __playerId);
        return __playerId;
    }

    public CustomProtocolSession GetSession(int vPlayerId)
    {
        CustomProtocolSession session = null;
        _playerId2Session.TryGetValue(vPlayerId, out session);
        return session;
    }

    public void SetMoneyRaw(int vPlayerId, int vMoney)
    {
        GetPlayerInfo(vPlayerId).m_Money = vMoney;
    }

    public void SetCoinRaw(int vPlayerId, int vCoin)
    {
        GetPlayerInfo(vPlayerId).m_Coin = vCoin;
    }

    public int GetMoney(int vPlayerId)
    {
        return GetPlayerInfo(vPlayerId).m_Money;
    }

    public int GetCoin(int vPlayerId)
    {
        return GetPlayerInfo(vPlayerId).m_Coin;
    }
}