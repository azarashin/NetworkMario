using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    RoomList _roomList = new RoomList();

    void Start()
    {
        _roomList.Clear();

    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        _roomList.Update(changedRoomList);
    }

    public override void OnLeftLobby()
    {
        _roomList.Clear();
    }

    public RoomList GetRoomList()
    {
        return _roomList; 
    }

}
