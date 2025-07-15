using System;
using Fusion;
using UnityEngine;

public class NetworkStyle : NetworkBehaviour
{
    private MeshRenderer buttonRenderer;
    public Color OffColor = Color.red;
    public Color OnColor = Color.green;
    public bool isOn;

    public override void Spawned()
    {
        buttonRenderer = GetComponent<MeshRenderer>();
        isOn = false; // Initialize the button state to Off
        SetButtonColor(OffColor); // Set the initial color
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ToggleButton()
    {
        isOn = !isOn; // Toggle the button state
        SetButtonColor(isOn ? OnColor : OffColor); // Update the color based on the state
        NetworkStyleManager.Instance.RPC_CheckStyle(); // Notify the manager to check the style
    }

    private void SetButtonColor(Color color)
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = color; // Change the button color
        }
        else
        {
            Debug.LogWarning("Button renderer is not assigned.");
        }
    }
}
