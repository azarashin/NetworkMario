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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PhotonController.Start");
        _connectedToMaster.SetActive(false);
        _joinedRoom.SetActive(false); 

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
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
        _panel.SetActive(false); 
        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
