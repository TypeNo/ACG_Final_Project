using Fusion;
using UnityEngine;

public class NetworkPressurePlate : NetworkBehaviour
{
    public MeshRenderer buttonMeshRenderer;
    public Color pressedColor = Color.red;
    public Color defaultColor;
    [Networked, OnChangedRender(nameof(ApplyColorChange))]
    public Color ButtonColor { get; set; }
    public GameObject doorToOpenLeft;
    public GameObject doorToOpenRight;

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
        OpenDoor();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeactivateButton()
    {
        if (!isPressed) return; // Only deactivate if it was pressed
        ChangeColor(defaultColor);
        isPressed = false;
        CloseDoor();
    }

    void OpenDoor()
    {
        //left door -90 means open, 0 means closed
        //right door 90 means open, 0 means closed
        if (doorToOpenLeft != null)
        {
            doorToOpenLeft.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        if (doorToOpenRight != null)
        {
            doorToOpenRight.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
    }

    void CloseDoor()
    {
        //left door -90 means open, 0 means closed
        //right door 90 means open, 0 means closed
        if (doorToOpenLeft != null)
        {
            doorToOpenLeft.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (doorToOpenRight != null)
        {
            doorToOpenRight.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
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
}
