using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    public static LobbyManager Instance { get; private set; }

    [Networked, Capacity(4), OnChangedRender(nameof(OnPlayersChanged))]
    public NetworkLinkedList<LobbyPlayerData> Players { get; } = default;
    [SerializeField] private NetworkObject _lobbyPlayerPrefab;
    [SerializeField] private LobbyPlayerListDataEvent _onPlayerListChanged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Spawned()
    {
        OnPlayersChanged();
    }

    private void SpawnPlayerData(PlayerRef player)
    {
        var playerObj = Runner.Spawn(_lobbyPlayerPrefab, position: Vector3.zero, inputAuthority: player);
        playerObj.name = "LobbyPlayer_" + Players.Count + 1;

        Players.Add(playerObj.GetComponent<LobbyPlayerData>());
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            SpawnPlayerData(player);
            Debug.Log($"Player {player} joined the lobby. Total players: {Players.Count}");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            LobbyPlayerData playerData = null;
            foreach (var p in Players)
            {
                if (p.playerRef == player)
                {
                    playerData = p;
                    break;
                }
            }
            if (playerData != null)
            {
                Players.Remove(playerData);
                Runner.Despawn(playerData.Object);
            }
        }
    }

    public void StartGame()
    {
        if (Runner.IsServer && Players.Count > 0)
        {
            Runner.SessionInfo.IsOpen = false; // Lock the session
            Debug.Log("Starting game with " + Players.Count + " players.");
            SceneRef gameScene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{GameConfig.GAME_SCENE}.unity"));
            Runner.LoadScene(gameScene, LoadSceneMode.Single);
        }
    }

    private void OnPlayersChanged()
    {
        _onPlayerListChanged.Raise(Players);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
