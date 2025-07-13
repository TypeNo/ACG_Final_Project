using Fusion;
using Fusion.Photon.Realtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyLogic : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab;
    [SerializeField] private LobbyManager _lobbyManagerPrefab;
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private NetworkRunner _runner;

    public async Task<StartGameResult> CreateRoom(string roomId)
    {
        _runner = Instantiate(_networkRunnerPrefab);

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = roomId,
            PlayerCount = maxPlayers,
            SceneManager = _runner.GetComponent<INetworkSceneManager>() ??
                      _runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok && _runner.IsServer)
        {
            SpawnLobbyManager();
        }

        return result;
    }

    public async Task<StartGameResult> JoinRoom(string roomId)
    {
        _runner = Instantiate(_networkRunnerPrefab);
        return await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = roomId
        });
    }

    private void SpawnLobbyManager()
    {
        if (_runner.IsServer && _lobbyManagerPrefab != null)
        {
            _runner.Spawn(_lobbyManagerPrefab, Vector3.zero, Quaternion.identity);
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
