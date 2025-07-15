using Fusion;
using TMPro;
using UnityEngine;

public class PasswordVerification : NetworkBehaviour
{
    public GameObject verifyPanel;
    public TMP_InputField inputField; // The input field for entering the password
    public TextMeshProUGUI verificationResultText; // The text component to display the verification result
    public Transform[] leftDoor;
    public Transform[] rightDoor;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player.StateAuthority())
            {
                verifyPanel.SetActive(true); // Show the verification panel
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player.StateAuthority())
            {
                verifyPanel.SetActive(false); // Hide the verification panel
            }
        }
    }

    public void VerifyPassword()
    {
        Debug.Log("Verifying password: " + inputField.text); Debug.Log("Expected password: " + GameManager.Instance.Password);
        Debug.Log("Comparing: " + GameManager.Instance.Password == inputField.text);
        if (GameManager.Instance.Password == inputField.text)
        {
            verificationResultText.text = "Password Verified!";
            // Additional logic for successful verification can be added here
            RPC_OpenDoor(); // Open the doors for all players
        }
        else
        {
            verificationResultText.text = "Incorrect Password!";
            // Additional logic for failed verification can be added here
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
