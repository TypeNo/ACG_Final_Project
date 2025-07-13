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

        if (Runner.IsServer)
        {
            RPC_SetPlayerRole(PlayerRole.PLAYER1);
            Runner.MakeDontDestroyOnLoad(gameObject);
        }
        else
        {
            RPC_SetPlayerRole(PlayerRole.PLAYER2);
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
