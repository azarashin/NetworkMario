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
    Text _info;

    GameObject[] _spawnPoints;

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

        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("PhotonController.OnJoinedRoom");
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
