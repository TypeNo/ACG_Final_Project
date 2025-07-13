using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkObject PlayerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    public NetworkDictionary<PlayerRef, Player> SpawnedPlayers { get; set; }
    private List<PlayerRef> joinOrder = new List<PlayerRef>();
    public bool isTesting = false;

    public void PlayerJoined(PlayerRef player)
    {

        // if (player == Runner.LocalPlayer)
        // {
        //     Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        // }

        Debug.Log($"Player {player} joined the game. Total players: {_spawnedPlayers.Count} + {Runner.IsServer}");
        //SpawnPlayer(player);
        // if (Runner.IsServer)
        // {
        //     Runner.Spawn(inGamePlayerManagerPrefab, position: Vector3.zero, inputAuthority: player);
        //     Runner.Spawn(gameInitializerPrefab, position: Vector3.zero, inputAuthority: player);
        // }
    }

    public void SpawnPlayer(PlayerRef player)
    {
        if (!Runner.IsServer)
        {
            Debug.Log("Not server, skipping spawn.");
            return;
        }

        Debug.Log($"Spawning player {player}");
        
        NetworkObject playerObj = Runner.Spawn(
            PlayerPrefab,
            new Vector3(0, 1, 0),
            Quaternion.identity,
            inputAuthority: player
        );

        if (playerObj == null)
        {
            Debug.LogError("Player object failed to spawn!");
            return;
        }

        Debug.Log("Player spawned successfully!");

        playerObj.name = $"PlayerObject_{player}";
        _spawnedPlayers[player] = playerObj;

        Runner.SetPlayerObject(player, playerObj);
    }

    public void PlayerLeft(PlayerRef player)
    {

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
