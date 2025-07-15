using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public NetworkWinPressurePlate[] winPressurePlate;
    public GameObject winPanel;

    [Networked]
    public string Password { get; set; } // The game password

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
        if (!HasStateAuthority)
            return;
        SetRandomPassword(); // Set a random password when the game starts
    }

    void SetRandomPassword()
    {
        Debug.Log("Setting random password");
        // Generate a random password
        Password = Random.Range(1000, 9999).ToString();
    }

    public void CheckWinCondition()
    {
        bool allButtonsPressed = true;
        foreach (var pressurePlate in winPressurePlate)
        {
            Debug.Log($"Checking pressure plate: {pressurePlate.name}, isPressed: {pressurePlate.isButtonPressed()}");
            if (!pressurePlate.isButtonPressed())
            {
                allButtonsPressed = false;
                break;
            }
        }

        if (allButtonsPressed)
        {
            if (Object.HasStateAuthority)
            {
                RPC_WinGame(); // Notify all clients that the game is won
            }

        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_WinGame()
    {
        Debug.Log("All buttons pressed! You win!");
        // Implement win logic here, such as showing a win screen or transitioning to a new scene
        
        winPanel.SetActive(true); // Show the win panel
    }
}
