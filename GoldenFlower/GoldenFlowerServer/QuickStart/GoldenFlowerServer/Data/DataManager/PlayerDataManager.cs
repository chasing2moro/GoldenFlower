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
    public void AddPlayer(CustomProtocolSession vSession, int vPlayerId)
    {
        _session2PlayerId[vSession] = vPlayerId;
        _playerId2Session[vPlayerId] = vSession;
    }
    public void RemovePlayer(CustomProtocolSession vSession)
    {
        if (_session2PlayerId.ContainsKey(vSession))
        {
            //反向列表先移除
            _playerId2Session.Remove(_session2PlayerId[vSession]);

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
}

