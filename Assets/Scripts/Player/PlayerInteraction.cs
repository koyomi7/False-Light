using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactionDistance;
    [SerializeField] float throwForce;

    [Header("References")]
    [SerializeField] Transform holdPoint;
    [SerializeField] Camera playerCam;
    [SerializeField] TMPro.TextMeshProUGUI interactionText;
    [SerializeField] LayerMask interactableLayer;

    GameObject heldObject;
    IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractable();
        HandleInteraction();
    }

    public void Pickup(GameObject obj)
    {
        if (heldObject != null) return;

        obj.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
        obj.GetComponent<Collider>().enabled = false; // Disable collision

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        heldObject = obj;
        currentInteractable = null;
        interactionText.SetText("[F] to drop\n[LMB] to throw");
        Debug.Log($"Player picked up {heldObject.name}");
    }

    public void Drop(bool _throw = false)
    {
        if (heldObject == null) return;
        
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false; // Re-enable physics
        heldObject.GetComponent<Collider>().enabled = true; // Re-enable collision

        if (_throw) rb.AddForce(holdPoint.forward * throwForce, ForceMode.Impulse); // Throw
        heldObject.transform.SetParent(null); // Unparent
        Debug.Log(_throw ? $"Player threw {heldObject.name}" : $"Player dropped {heldObject.name}");
        heldObject = null;
    }

    void CheckForInteractable()
    {
        if (heldObject != null) return;
        
        // Gets camera's pitch angle from local rotation
        float pitch = playerCam.transform.localEulerAngles.x;
        pitch = (pitch > 180) ? pitch - 360 : pitch; // Normalize to -180 to 180 range

        // Calculates distance multiplier based on pitch (0° at horizon, max at ±90°)
        float pitchMultiplier = 1 + Mathf.Abs(pitch) / 90f; // Ranges from 1 to 2
        float dynamicDistance = interactionDistance * pitchMultiplier;

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * dynamicDistance, Color.green);

        // Gets all hits in the ray path
        RaycastHit[] hits = Physics.RaycastAll(ray, dynamicDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // Resets interactable at start
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);

        foreach (RaycastHit hit in hits)
        {
            // Obstacle blocks everything behind it
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("InteractionClip"))
            {
                return;
            }

            // Check for interactables
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) continue; // neither obstacle nor interactable -> continue to next hit

            // Found an interactable
            currentInteractable = interactable;

            // Access Mechanism
            if (hit.collider.CompareTag("AccessMechanism"))
            {
                GenericAccessMechanismScript access = hit.collider.GetComponent<GenericAccessMechanismScript>();
                bool accessible = !access.isOnCooldown;
                bool closed = access.state == GenericAccessMechanismScript.states.CLOSED ? true : false;
                interactionText.SetText(closed ? "[F] to open" : "[F] to close");
                if (accessible) interactionText.gameObject.SetActive(true);
            }
            // Interactive Prop
            else if (hit.collider.CompareTag("InteractiveProp"))
            {
                interactionText.SetText("[F] to pickup");
                interactionText.gameObject.SetActive(true);
            }

            return; // don't check further objects
        }
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject != null)
            {
                Drop();
            }
            else if (currentInteractable != null)
            {
                // Check if the interactable is a door requiring a key
                GenericAccessMechanismScript door = currentInteractable as GenericAccessMechanismScript;
                if (door != null && door.requiresKey && !door.isUnlocked)
                {
                    // if (!KeyInventory.Instance.HasKey())
                    // {
                    //     ShowKeysNeededText();
                    //     KeysAudioSource.clip = OpenDoorFailedSound;
                    //     KeysAudioSource.Play();
                    // }
                    // else
                    // {
                    currentInteractable.Interact();
                    //     KeysAudioSource.clip = UnlockDoorSound;
                    //     KeysAudioSource.Play();
                    // }
                }
                else
                {
                    currentInteractable.Interact();
                }

            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null)
            {
                Drop(true);
            }
        }
    }
}
