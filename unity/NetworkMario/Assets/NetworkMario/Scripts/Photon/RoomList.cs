using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

public class RoomList : IEnumerable<RoomInfo>
{
    private Dictionary<string, RoomInfo> _dictionary = new Dictionary<string, RoomInfo>();

    public void Update(List<RoomInfo> changedRoomList)
    {
        foreach (var info in changedRoomList)
        {
            if (!info.RemovedFromList)
            {
                _dictionary[info.Name] = info;
            }
            else
            {
                _dictionary.Remove(info.Name);
            }
        }
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    public bool TryGetRoomInfo(string roomName, out RoomInfo roomInfo)
    {
        return _dictionary.TryGetValue(roomName, out roomInfo);
    }

    public IEnumerator<RoomInfo> GetEnumerator()
    {
        foreach (var kvp in _dictionary)
        {
            yield return kvp.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}