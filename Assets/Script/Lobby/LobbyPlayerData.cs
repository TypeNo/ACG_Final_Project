using System;
using Fusion;
using UnityEngine;
public class LobbyPlayerData : NetworkBehaviour
{
    [Networked] public PlayerRef playerRef { get; set; }
    [Networked] public NetworkString<_32> Name { get; set; }
    [Networked] public PlayerRole Role { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isInitialized = false;

    public override void Spawned()
    {
        playerRef = Object.InputAuthority;

        if (Object.HasInputAuthority)
        {
            // Called by local player who owns this object
            var nickname = PlayerPrefs.GetString("PlayerName", "Player");
            RPC_SetNickname(nickname);

            // Assign role based on logic (optional)
            if (Runner.IsServer)
                RPC_SetPlayerRole(PlayerRole.PLAYER1);
            else
                RPC_SetPlayerRole(PlayerRole.PLAYER2);
        }

        if (Runner.IsServer)
        {
            Runner.MakeDontDestroyOnLoad(gameObject);
        }

        isInitialized = true;
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetNickname(NetworkString<_32> nickname)
    {
        Name = nickname;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerRole(PlayerRole role)
    {
        Role = role;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
