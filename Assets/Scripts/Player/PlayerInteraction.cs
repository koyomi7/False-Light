using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactionDistance;
    [SerializeField] float throwForce;

    [Header("Sounds")]
    [SerializeField] AudioClip openDoorFailSound;
    [SerializeField] AudioClip doorUnlockSound;
    AudioSource keyAudioSource;

    [Header("References")]
    [SerializeField] Transform holdPoint;
    [SerializeField] Camera playerCam;
    [SerializeField] TMPro.TextMeshProUGUI interactionText;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] TMPro.TextMeshProUGUI keysNeededText; // NEW: Reference to the "Keys Needed" text

    GameObject heldObject;
    Quaternion initialRotationOffset;
    IInteractable currentInteractable;

    // Timer for "Keys Needed" visibility
    float keysNeededTimer = 0f;
    const float KeysNeededDisplayTime = 2f; // Time in seconds to show "Keys Needed"

    void Start()
    {
        keyAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckForInteractable();
        HandleInteraction();
        UpdateKeysNeededVisibility();
    }

    void LateUpdate()
    {
        if (heldObject != null)
        {
            // Get the camera's Y rotation (yaw) only (ignoring pitch & roll)
            float cameraYaw = playerCam.transform.eulerAngles.y;
            Quaternion yawOnlyRotation = Quaternion.Euler(0, cameraYaw, 0);

            // Apply the yaw rotation + initial offset to the object
            heldObject.transform.rotation = yawOnlyRotation * initialRotationOffset;

            // Update position to follow holdPoint
            heldObject.transform.position = holdPoint.position;
        }
    }

    public void Pickup(GameObject obj)
    {
        if (heldObject != null) return;

        obj.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
        obj.GetComponent<Collider>().enabled = false; // Disable collision

        // obj.transform.SetParent(holdPoint);
        // obj.transform.localPosition = Vector3.zero;
        // obj.transform.localRotation = Quaternion.identity;

        Quaternion worldRotation = obj.transform.rotation;

        // Removes the camera's yaw influence from the initial offset
        float cameraYaw = playerCam.transform.eulerAngles.y;
        Quaternion inverseYaw = Quaternion.Euler(0, -cameraYaw, 0);
        initialRotationOffset = inverseYaw * worldRotation;

        heldObject = obj;
        currentInteractable = null;
        interactionText.SetText("[F] Drop\n[LMB] Throw");
        Debug.Log($"Player picked up {heldObject.name}");
    }

    public void Drop(bool _throw = false)
    {
        if (heldObject == null) return;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false; // Re-enable physics
        heldObject.GetComponent<Collider>().enabled = true; // Re-enable collision

        if (_throw) rb.AddForce(holdPoint.forward * throwForce, ForceMode.Impulse); // Throw
        // heldObject.transform.SetParent(null); // Unparent
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
                // Debug.Log("Hit Obstacle");
                return;
            }

            // Check for interactables
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) continue; // neither obstacle nor interactable -> continue to next hit

            // Found an interactable
            currentInteractable = interactable;

            // Access Mechanism
            if (hit.collider.CompareTag("AccessMechanism") || hit.collider.CompareTag("Printer"))
            {
                GenericAccessMechanismScript access = hit.collider.GetComponent<GenericAccessMechanismScript>();
                bool accessible = !access.isOnCooldown;
                if (hit.collider.CompareTag("Printer")) interactionText.SetText("[F] Roll");
                else
                {
                    bool closed = access.state == GenericAccessMechanismScript.states.CLOSED ? true : false;
                    interactionText.SetText(closed ? "[F] Open" : "[F] Close");
                }
                if (accessible) interactionText.gameObject.SetActive(true);
            }
            // Interactive Prop
            else if (hit.collider.CompareTag("InteractiveProp"))
            {
                interactionText.SetText("[F] Pickup");
                interactionText.gameObject.SetActive(true);
            }
            // Light Switch
            else if (hit.collider.CompareTag("LightSwitch"))
            {
                LightSwitchScript lightSwitch = hit.collider.GetComponent<LightSwitchScript>();
                bool off = !lightSwitch.state;
                interactionText.SetText(off ? "[F] Turn On" : "[F] Turn Off");
                interactionText.gameObject.SetActive(true);
            }
            // Pills
            else if (hit.collider.CompareTag("Pill"))
            {
                interactionText.SetText("[F] Consume");
                interactionText.gameObject.SetActive(true);
            }
            // Key
            else if (hit.collider.CompareTag("Key"))
            {
                interactionText.SetText("[F] Grab Key");
                interactionText.gameObject.SetActive(true);
            }

            return; // don't check further objects
        }
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject != null) Drop();
            else if (currentInteractable != null)
            {
                // Check if the interactable is a door requiring a key
                GenericAccessMechanismScript door = currentInteractable as GenericAccessMechanismScript;
                if (door != null && door.requiresKey && !door.isUnlocked)
                {
                    if (!KeyInventory.Instance.HasKey())
                    {
                        ShowKeysNeededText();
                        keyAudioSource.clip = openDoorFailSound;
                    }
                    else
                    {
                        currentInteractable.Interact();
                        keyAudioSource.clip = doorUnlockSound;
                    }
                    keyAudioSource.Play();
                }
                else currentInteractable.Interact();

            }
        }
        if (Input.GetMouseButtonDown(0) && (heldObject != null)) Drop(true);
    }
    
    void ShowKeysNeededText()
    {
        if (keysNeededText != null)
        {
            keysNeededText.gameObject.SetActive(true);
            keysNeededTimer = KeysNeededDisplayTime; // Reset timer
        }
    }

    void UpdateKeysNeededVisibility()
    {
        if (keysNeededText != null && keysNeededText.gameObject.activeSelf)
        {
            keysNeededTimer -= Time.deltaTime;
            if (keysNeededTimer <= 0f)
            {
                keysNeededText.gameObject.SetActive(false); // Hide after timer expires
            }
        }
    }
}
