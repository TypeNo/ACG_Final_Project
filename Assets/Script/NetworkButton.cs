using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButton : NetworkBehaviour
{
    public CanvasGroup canvasGroup; // The UI panel to show
    public float minDistance = 0.5f;  // Fully visible when this close
    public float maxDistance = 3.0f;  // Fully invisible beyond this
    public Transform leftDoor;
    public Transform rightDoor;
    private bool isDoorOpen = false;

    private Transform playerTransform;
    private bool playerInRange = false;

    public override void Spawned()
    {
        canvasGroup.alpha = 0f; // Start invisible
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Player>().StateAuthority())
            return;
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerInRange = false;
            canvasGroup.alpha = 0f;
        }
    }

    private void Update()
    {
        if (playerInRange && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= minDistance)
            {
                canvasGroup.alpha = 1f; // Fully visible
            }
            else if (distance >= maxDistance)
            {
                canvasGroup.alpha = 0f; // Fully invisible
            }
            else
            {
                // Linear fade between min and max distance
                float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
                canvasGroup.alpha = t;
            }
        }
    }

    private void ToggleDoor()
    {
        if (!isDoorOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Interact()
    {
        ToggleDoor();
    }

    public void OpenDoor()
    {
        if (leftDoor != null)
        {
            leftDoor.localRotation = Quaternion.Euler(0, -0, -90); // Open left door
        }

        if (rightDoor != null)
        {
            rightDoor.localRotation = Quaternion.Euler(0, 0, 90); // Open right door
        }

        isDoorOpen = true;
    }

    public void CloseDoor()
    {
        if (leftDoor != null)
        {
            leftDoor.localRotation = Quaternion.Euler(0, 0, 0); // Close left door
        }

        if (rightDoor != null)
        {
            rightDoor.localRotation = Quaternion.Euler(0, 0, 0); // Close right door
        }

        isDoorOpen = false;
    }
}


