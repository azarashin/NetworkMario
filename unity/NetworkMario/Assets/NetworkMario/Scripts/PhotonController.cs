using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonController : Fusion.Behaviour, INetworkRunnerCallbacks
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 Force;
        public Vector2 Impulse;
        public bool TriggerJump; 
            
    }

    [SerializeField]
    GameObject _panel; 

    [SerializeField]
    Text _info;

    [SerializeField]
    GameObject _startButton; 

    [SerializeField]
    NetworkPrefabRef _playerPrefab;

    Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    GameObject[] _spawnPoints;
    NetworkObject _networkPlayerObject;

    // Start is called before the first frame update
    void Start()
    {
        _startButton.SetActive(false);
        StartGame(GameMode.AutoHostOrClient); 
    }

    void Update()
    {
        string info = "";
///        info += RoomInfo();

        _info.text = info; 
    }

    string RoomInfo()
    {
        string info = "";
        NetworkRunner runner = GetComponent<NetworkRunner>();
        if (runner.SessionInfo != null)
        {
            info += $"runner.SessionInfo.Name: {runner.SessionInfo.Name}";
            info += $"runner.SessionInfo.PlayerCount: {runner.SessionInfo.PlayerCount}\n";
            
            if(runner.ActivePlayers != null)
            {
                foreach (var player in runner.ActivePlayers)
                {
                    info += $"  {player.ToString()}\n";
                }
            }
        }
        else
        {
            info += $"Your are not in any room...\n";
        }
        return info; 
    }



    public void OnStartGame()
    {
        Debug.Log("PhotonController.OnStartGame");
        NetworkRunner runner = GetComponent<NetworkRunner>();
        if(_spawnedCharacters[runner.LocalPlayer].HasInputAuthority)
        {
            RpcRequestStartGameToMasterClient();
            float k = 0;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcRequestStartGameToMasterClient(RpcInfo info = default)
    {
        Debug.Log("PhotonController.RpcRequestStartGameToMasterClient");
        if (IsMasterPlayer())
        {
            int id = 0;
            foreach (var player in _spawnedCharacters.Keys)
            {
                //                RpcStartGame(player.PlayerId, _spawnPoints[id].transform.position); 
                _networkPlayerObject.transform.position = _spawnPoints[id].transform.position;
                _networkPlayerObject.gameObject.SetActive(true); 
                id = (id + 1) % _spawnPoints.Length;
            }
        }
        _panel.SetActive(false);
    }

    private PlayerRef MasterPlayer()
    {
        int master_id = _spawnedCharacters.Keys.Min(s => s.PlayerId);
        return _spawnedCharacters.Keys.Where(s => s.PlayerId == master_id).FirstOrDefault();
    }

    private bool IsMasterPlayer()
    {
        PlayerRef master_player_ref = MasterPlayer();
        return !master_player_ref.IsNone && master_player_ref.IsValid && _spawnedCharacters[master_player_ref] == _networkPlayerObject; 
    }

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        NetworkRunner runner = GetComponent<NetworkRunner>();
        runner.ProvideInput = true;

        Debug.Log("PhotonController.StartGame[0]");

        // Start or join (depends on gamemode) a session with a specific name
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerBase>()
        });

        Debug.Log("PhotonController.StartGame[1]");

        _spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        Debug.Log($"PhotonController.Start: number of spawns={_spawnPoints.Length}");

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("PhotonController.OnPlayerJoined");
        // Create a unique position for the player
        _networkPlayerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
        // Keep track of the player avatars so we can remove it when they disconnect
        _spawnedCharacters.Add(player, _networkPlayerObject);

        _networkPlayerObject.gameObject.SetActive(false);
        if (_networkPlayerObject.HasInputAuthority)
        {
            Debug.Log("PhotonController.OnPlayerJoined: You joined");
            _startButton.SetActive(true);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("PhotonController.OnPlayerLeft");
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
        if (player == runner.LocalPlayer)
        {
            Debug.Log("PhotonController.OnPlayerJoined: You left");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(_networkPlayerObject == null)
        {
            return; 
        }

        var data = new NetworkInputData();

        Player player = _networkPlayerObject.GetComponent<Player>(); 
        if(player == null)
        {
            return; 
        }

        data.Force = Vector2.zero;
        data.Impulse = Vector2.zero;
        data.TriggerJump = false; 
        if (Input.GetKey(KeyCode.A))
        {
            data.Force += Vector2.left * player.MoveSpeed();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            data.Force += Vector2.right * player.MoveSpeed();
        }
        if (Input.GetKey(KeyCode.W) && player.IsOnFloor())
        {
            data.Impulse += Vector2.up * player.JumpImpulse();
            data.TriggerJump = true; 
        }

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("PhotonController.OnInputMissing");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("PhotonController.OnShutdown");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("PhotonController.OnConnectToServer");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("PhotonController.OnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("PhotonController.OnConnectRequest");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("PhotonController.OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("PhotonController.OnUserSimulationMessage");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("PhotonController.OnSessionListUpdated");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("PhotonController.OnCustomAuthenticationResponse");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("PhotonController.OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("PhotonController.OnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("PhotonController.OnSceneLoadStart");
    }
}
