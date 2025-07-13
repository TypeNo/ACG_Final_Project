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
    [Header("Player Stage")]
    [SerializeField] private Transform playerStageRoot;
    [SerializeField] private GameObject playerStageDisplayPrefab;
    [SerializeField] private float spacing = 2.5f;
    private List<GameObject> playerDisplays = new List<GameObject>();

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

        playerStageRoot = GameObject.Find("PlayerStage")?.transform;

        if (playerStageRoot == null)
        {
            Debug.LogError("PlayerStageRoot not found in the hierarchy.");
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

    #region  OnStartGameOrReadyClicked
    public void OnStartGameOrReadyClicked()
    {
        if (Runner.IsServer)
        {
            // if (!CheckAllPlayersReady())
            // {
            //     Debug.Log("Not all players are ready. Cannot start the game.");
            //     return;
            // }
            StartGame();
        }
        else
        {
            //TogglePlayerReady();
        }
    }
    #endregion

    private void OnPlayersChanged()
    {
        //_onPlayerListChanged.Raise(Players);
            // Clear previous displays
    foreach (var go in playerDisplays)
        Destroy(go);
    playerDisplays.Clear();

    // Get the current players (example: Players is a list or dictionary)
    var players = Players; // or LobbyManager.Instance.Players if needed

    for (int i = 0; i < players.Count; i++)
    {
        var player = players[i];

        // Instantiate the display
        var display = Instantiate(playerStageDisplayPrefab, playerStageRoot);

        // Position at a 45-degree incline
        Vector3 position = new Vector3(i * spacing, 0, i * spacing); // 45° incline
        display.transform.localPosition = position;

        // Optional: Set name or appearance
        display.name = $"PlayerDisplay_{i}";
        //display.GetComponentInChildren<TextMeshProUGUI>()?.SetText(player.Name); // if you have a name tag

        playerDisplays.Add(display);
    }

        
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
