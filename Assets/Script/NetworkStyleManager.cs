using Fusion;
using UnityEngine;

public class NetworkStyleManager : NetworkBehaviour
{
    public static NetworkStyleManager Instance;
    Color OffColor = Color.red;
    Color OnColor = Color.green;
    public MeshRenderer[] buttonRenderers;
    public NetworkStyle[] leftNetworkStylesButtons;
    public NetworkStyle[] rightNetworkStylesButtons;
    public Transform[] leftDoor;
    public Transform[] rightDoor;

    [Networked, Capacity(5)]
    private NetworkArray<bool> buttonlist { get; }
    private int maxButtonCount = 5;

    void Awake()
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
        if (Object.HasStateAuthority)
        {
            // Initialize the style for the local player
            InitializeStyle();
        }
    }

    void InitializeStyle()
    {
        for (int i = 0; i < maxButtonCount; i++)
        {
            buttonlist.Set(i, false); // Initialize all buttons to Off
        }
        //Randomly set the first button to OnColor
        for (int i = 0; i < maxButtonCount; i++)
        {
            if (Random.Range(0, 2) == 1)
            {
                buttonlist.Set(i, true);
                SetButtonColor(i, OnColor);
            }
        }

        for (int i = 0; i < maxButtonCount; i++)
        {
            Debug.Log($"Button {i} is {(buttonlist[i] ? "On" : "Off")}");
        }
    }

    void SetButtonColor(int buttonId, Color color)
    {
        if (buttonId < 0 || buttonId >= buttonRenderers.Length)
        {
            Debug.LogError("Button ID out of range: " + buttonId);
            return;
        }

        if (buttonRenderers[buttonId] != null)
        {
            buttonRenderers[buttonId].material.color = color;
        }
        else
        {
            Debug.LogError("Button renderer is null for ID: " + buttonId);
        }
    }

    public void GetButtonListState()
    {
        for (int i = 0; i < maxButtonCount; i++)
        {
            Debug.Log($"Button {i} is {(buttonlist[i] ? "On" : "Off")}");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_CheckStyle()
    {
        bool correctStyle = true;
        for (int i = 0; i < maxButtonCount; i++)
        {
            if (!(buttonlist[i] == leftNetworkStylesButtons[i].isOn && buttonlist[i] == rightNetworkStylesButtons[i].isOn))
            {
                correctStyle = false;
            }
        }

        if (correctStyle)
        {
            Debug.Log("Correct style applied!");
            RPC_OpenDoor(); // Open the doors for all players
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_OpenDoor()
    {
        OpenDoor(); // Open the doors for all players
    }

    void OpenDoor()
    {
        foreach (Transform doorToOpenLeft in leftDoor)
        {
            if (doorToOpenLeft != null)
            {
                doorToOpenLeft.transform.localRotation = Quaternion.Euler(0, 0, -90); // Open left door
            }
        }

        foreach (Transform doorToOpenRight in rightDoor)
        {
            if (doorToOpenRight != null)
            {
                doorToOpenRight.transform.localRotation = Quaternion.Euler(0, 0, 90); // Open right door
            }
        }
    }

    void CloseDoor()
    {
        foreach (Transform doorToOpenLeft in leftDoor)
        {
            if (doorToOpenLeft != null)
            {
                doorToOpenLeft.transform.localRotation = Quaternion.Euler(0, 0, 0); // Close left door
            }
        }

        foreach (Transform doorToOpenRight in rightDoor)
        {
            if (doorToOpenRight != null)
            {
                doorToOpenRight.transform.localRotation = Quaternion.Euler(0, 0, 0); // Close right door
            }
        }
    }
}
