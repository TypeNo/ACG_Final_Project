using Fusion;
using UnityEngine;

public class NetworkWinPressurePlate : NetworkBehaviour
{
    public MeshRenderer buttonMeshRenderer;
    public Color pressedColor = Color.green;
    public Color defaultColor;
    [Networked, OnChangedRender(nameof(ApplyColorChange))]
    public Color ButtonColor { get; set; }

    private bool isPressed = false;
    public override void Spawned()
    {
        defaultColor = buttonMeshRenderer.material.color;
    }

    public void ChangeColor(Color newColor)
    {
        ButtonColor = newColor;
    }

    public void ApplyColorChange()
    {
        buttonMeshRenderer.material.color = ButtonColor;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PressButton()
    {
        if (isPressed) return; // Prevent multiple presses

        ChangeColor(pressedColor);
        isPressed = true;

        GameManager.Instance.CheckWinCondition(); // Check if all buttons are pressed
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeactivateButton()
    {
        if (!isPressed) return; // Only deactivate if it was pressed
        ChangeColor(defaultColor);
        isPressed = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Player>().StateAuthority())
            return;
        if (isPressed)
        {
            RPC_DeactivateButton(); // Deactivate the button when the player exits the trigger
        }
    }

    public bool isButtonPressed()
    {
        return isPressed; // Return the current state of the button
    }
}
