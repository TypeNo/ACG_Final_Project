using Fusion;
using TMPro;
using UnityEngine;

public class NetworkPassword : NetworkBehaviour
{
    public GameObject passwordPanel; // The UI panel to show
    public TextMeshProUGUI passwordText; // The text component to display the password
    public TextMeshProUGUI receivedPasswordText; // The text component to display the received password
    public TMP_InputField inputField; // The input field for entering the password

    [Networked, OnChangedRender(nameof(OnReceivedChanged))]
    public string ReceivedPassword { get; set; } // The received password string
    public float minDistance = 0.5f;  // Fully visible when this close
    public float maxDistance = 3.0f;  // Fully invisible beyond this

    public override void Spawned()
    {
        passwordPanel.SetActive(false); // Start invisible

        passwordText.text = "Password: " + GameManager.Instance.Password; // Set the initial password text
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Player>().StateAuthority())
            return;
        if (other.CompareTag("Player"))
        {
            passwordPanel.SetActive(true); // Show the password panel
        }
        //Clear the input field when the player enters the trigger
        inputField.text = string.Empty;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Player>().StateAuthority())
            return;
        if (other.CompareTag("Player"))
        {
            passwordPanel.SetActive(false); // Hide the password panel
        }
    }

    public void RPC_SharePassword()
    {
        ReceivedPassword = inputField.text; // Set the received password from the input field
    }

    private void OnReceivedChanged()
    {
        receivedPasswordText.text = "Message from player: " + ReceivedPassword;
    }
}
