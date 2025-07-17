using System.Collections;
using Cinemachine;
using Fusion;
using StarterAssets;
using UnityEngine;


public class Player : NetworkBehaviour
{
    public float speed = 5f;
    public Animator animator;
    public ThirdPersonController _thirdPersonController;
    public StarterAssetsInputs _inputs;

    public Transform playerLookTransform;
    private CinemachineVirtualCamera _virtualCamera;
    private bool enableMovement = false;
    private NetworkButton nearbyButton;
    private NetworkPassword nearbyPassword;
    private PasswordVerification nearbyPasswordVerification;
    private PlayerRef playerRef;


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
            if (_virtualCamera != null)
            {
                _virtualCamera.Follow = playerLookTransform;
            }
        }

        StartCoroutine(Wait(1f)); // Wait for 1 second before enabling movement
    }

    void Update()
    {
        if (!HasStateAuthority)
            return;

        if (Input.GetKeyDown(KeyCode.E) && nearbyButton != null)
        {
            nearbyButton.RPC_Interact();
        }

        if (Input.GetKeyDown(KeyCode.Return) && nearbyPassword != null)
        {
            nearbyPassword.RPC_SharePassword(); // Share the password with all players
        }

        if (Input.GetKeyDown(KeyCode.Return) && nearbyPasswordVerification != null)
        {
            nearbyPasswordVerification.VerifyPassword();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NetworkStyleManager.Instance.GetButtonListState();
        }

        RayCastStyleButton();
    }

    public override void FixedUpdateNetwork()
    {
        if (!enableMovement)
            return;
        if (GetInput(out NetworkInputData inputData))
        {
            Vector2 move = inputData.move;
            bool jump = inputData.jump;
            bool sprint = inputData.sprint;

            _inputs.MoveInput(move);
            _inputs.JumpInput(jump);
            _inputs.SprintInput(sprint);

            _thirdPersonController.Move(Runner, sprint, move);
            _thirdPersonController.GroundedCheck();
            _thirdPersonController.JumpAndGravity(Runner);
        }
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enableMovement = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority)
            return;

        if (other.CompareTag("Pressure Plate"))
        {
            var networkPressurePlate = other.GetComponent<NetworkPressurePlate>();
            if (networkPressurePlate != null)
                networkPressurePlate.RPC_PressButton();
        }
        else if (other.CompareTag("Button"))
        {
            var button = other.GetComponent<NetworkButton>();
            if (button != null)
                nearbyButton = button; // remember the button
        }
        else if (other.CompareTag("Password"))
        {
            var password = other.GetComponent<NetworkPassword>();
            if (password != null)
                nearbyPassword = password; // remember the password
        }
        else if (other.CompareTag("Verification"))
        {
            var passwordVerification = other.GetComponent<PasswordVerification>();
            if (passwordVerification != null)
                nearbyPasswordVerification = passwordVerification; // remember the password verification
        }
        else if (other.CompareTag("WinPressurePlate"))
        {
            var winPressurePlate = other.GetComponent<NetworkWinPressurePlate>();
            if (winPressurePlate != null)
                winPressurePlate.RPC_PressButton(); // Press the button if it is not already pressed
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority)
            return;

        if (other.CompareTag("Button"))
        {
            var button = other.GetComponent<NetworkButton>();
            if (button == nearbyButton)
                nearbyButton = null;
        }
        else if (other.CompareTag("Password"))
        {
            var password = other.GetComponent<NetworkPassword>();
            if (password == nearbyPassword)
                nearbyPassword = null;
        }
        else if (other.CompareTag("Verification"))
        {
            var passwordVerification = other.GetComponent<PasswordVerification>();
            if (passwordVerification == nearbyPasswordVerification)
                nearbyPasswordVerification = null;
        }
    }

    public void RayCastStyleButton()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerLookTransform.position, playerLookTransform.forward, out hit, 1f))
        {
            if (hit.collider.CompareTag("StyleButton"))
            {
                Debug.DrawRay(playerLookTransform.position, playerLookTransform.forward * hit.distance, Color.yellow);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    var styleButton = hit.collider.GetComponent<NetworkStyle>();
                    styleButton?.RPC_ToggleButton(); // Toggle the button state
                }
            }
        }
        else
        {
            Debug.DrawRay(playerLookTransform.position, playerLookTransform.forward * 1f, Color.red);
        }
    }

    public bool InputAuthority()
    {
        return Object.HasInputAuthority;
    }

    public bool StateAuthority()
    {
        return Object.HasStateAuthority;
    }

    public PlayerRef GetPlayerRef()
    {
        return playerRef;
    }
}
