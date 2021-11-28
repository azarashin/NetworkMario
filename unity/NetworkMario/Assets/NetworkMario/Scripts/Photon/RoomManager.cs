using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    RoomList _roomList = new RoomList();

    void Start()
    {
        Debug.Log("RoomManager.Start");
        _roomList.Clear();

    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        base.OnRoomListUpdate(changedRoomList); 
        Debug.Log($"RoomManager.OnRoomListUpdate: length={changedRoomList.Count()}");
        _roomList.Update(changedRoomList);
        foreach(var room in changedRoomList)
        {
            Debug.Log($"RoomManager.OnRoomListUpdate: name={room.Name}, player_count={room.PlayerCount}");
        }
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby(); 
        Debug.Log("RoomManager.OnLeftLobby");
        _roomList.Clear();
    }

    public RoomList GetRoomList()
    {
        return _roomList; 
    }

}
