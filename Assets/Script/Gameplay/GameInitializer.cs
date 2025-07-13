using UnityEngine;
using Fusion;
using System.Collections;

public class GameInitializer : NetworkBehaviour
{
    [SerializeField] private PlayerSpawner _playerSpawner;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        // 1. Wait for all players to have their InGamePlayerData spawned
        InitializeWhenReady();
    }


    private void InitializeWhenReady()
    {
        //Spawn all players
        foreach (var player in Runner.ActivePlayers)
        {
            _playerSpawner.SpawnPlayer(player);
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
