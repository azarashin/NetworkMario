using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject _panel; 

    [SerializeField]
    GameObject _connectedToMaster;

    [SerializeField]
    GameObject _joinedRoom;

    [SerializeField]
    Text _info;

    GameObject[] _spawnPoints;

    public const string MAP_PROP_KEY = "map";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PhotonController.Start");
        _connectedToMaster.SetActive(false);
        _joinedRoom.SetActive(false); 

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
        return info; 
    }


    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("PhotonController.OnConnectedToMaster");
        _connectedToMaster.SetActive(true);
    }

    public void JoinRandomRoom(int max)
    {
        Debug.Log($"PhotonController.JoinRandomRoom{max}");

        byte mapCode = 0x00;
        byte expectedMaxPlayers = (byte)max;
        Hashtable expectedCustomRoomProperties = new Hashtable { { MAP_PROP_KEY, mapCode } };
        PhotonNetwork.JoinRandomOrCreateRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }

    public void JoinRoomMax2(string id)
    {
        Debug.Log($"PhotonController.JoinRoom{id}");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 2; 
        PhotonNetwork.JoinOrCreateRoom(id, roomOptions, TypedLobby.Default);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log($"PhotonController.JoinRoom");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        _connectedToMaster.SetActive(true);
        _joinedRoom.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Join Random Failed with error code {0} and error message {1}", returnCode, message);
        // here usually you create a new room
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("PhotonController.OnJoinedRoom");
        _connectedToMaster.SetActive(false);
        _joinedRoom.SetActive(true);

    }

    public void OnStartGame()
    {
        GetComponent<PhotonView>().RPC(nameof(RpcRequestStartGameToMasterClient), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RpcRequestStartGameToMasterClient()
    {
        if(PhotonNetwork.IsMasterClient)
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
        _panel.SetActive(false);
        if(id == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        }
    }
}
