using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject _panel; 

    [SerializeField]
    GameObject _connectedToMaster;

    [SerializeField]
    GameObject _joinedRoom;

    [SerializeField]
    RoomManager _roomManager; 

    [SerializeField]
    Text _info;

    GameObject[] _spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PhotonController.Start");
        _connectedToMaster.SetActive(false);
        _joinedRoom.SetActive(false);

        PhotonNetwork.LocalPlayer.NickName = ((int)Random.Range(0, 100)).ToString();

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

        _spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        Debug.Log($"PhotonCOntroller.Start: number of spawns={_spawnPoints.Length}");
    }

    void Update()
    {
        string info = "";
        info += RoomInfo();

        _info.text = info; 
    }

    string RoomInfo()
    {
        string info = "";
        if (PhotonNetwork.CurrentRoom != null)
        {
            info += $"PhotonNetwork.CurrentRoom.PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}\n";
            foreach(var player in PhotonNetwork.PlayerList)
            {
                info += $"  id: {player.ActorNumber}, name: {player.NickName}\n"; 
            }
        }
        else
        {
            info += $"Your are not in any room...\n";
        }
        RoomList roomList = _roomManager.GetRoomList();
        info += "RoomList:\n"; 
        foreach(RoomInfo room in roomList)
        {
            info += $"{room.ToStringFull()}\n";
        }
        return info; 
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster(); 
        Debug.Log("PhotonController.OnConnectedToMaster");
        _connectedToMaster.SetActive(true);

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("PhotonController.OnJoinedLobby");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)6;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    }


    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("PhotonController.OnJoinedRoom");
        _joinedRoom.SetActive(true);

    }

    public void OnStartGame()
    {
        Debug.Log("PhotonController.OnStartGame");
        GetComponent<PhotonView>().RPC(nameof(RpcRequestStartGameToMasterClient), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RpcRequestStartGameToMasterClient()
    {
        Debug.Log("PhotonController.RpcRequestStartGameToMasterClient");
        if (PhotonNetwork.IsMasterClient)
        {
            int id = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                GetComponent<PhotonView>().RPC(nameof(RpcStartGame), RpcTarget.All, player.ActorNumber, _spawnPoints[id].transform.position);
                id = (id + 1) % _spawnPoints.Length;
            }
        }
    }

    [PunRPC]
    private void RpcStartGame(int id, Vector3 pos)
    {
        Debug.Log("PhotonController.RpcStartGame");
        _panel.SetActive(false);
        if(id == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        }
    }
}
