using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour
{
    // Movement variables
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float lookXLimit;
    [SerializeField] private float defaultHeight;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchSpeed;

    // Stamina variables
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenRate = 0.5f;
    [SerializeField] private float staminaDepletionRate = 1f;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image staminaBarBackground;

    // Footstep sound variables
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    
    // Keys sound variables
    [SerializeField] private AudioClip OpenDoorFailedSound;
    [SerializeField] private AudioSource KeysAudioSource;
    [SerializeField] private AudioClip UnlockDoorSound;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool isMoving = false; // Track if the player is moving
    private bool isSprinting = false; // Track if the player is sprinting
    private bool isCrouching = false; // Track if the player is crouching

    // Timer for stamina bar visibility
    private float staminaFullTimer = 0f;
    private const float StaminaFullDelay = 2f; // Time in seconds before hiding the stamina bar

    // Crosshair and interaction variables
    [SerializeField] private Image crosshair; // Reference to the crosshair UI Image
    [SerializeField] private float interactionDistance = 1f; // Max distance for interaction
    [SerializeField] private LayerMask interactableLayer; // Layer for interactable objects
    [SerializeField] private LayerMask playerLayer; // Layer for player object
    [SerializeField] private TMPro.TextMeshProUGUI interactionText; // Reference to the interaction text
    [SerializeField] private TMPro.TextMeshProUGUI keysNeededText; // NEW: Reference to the "Keys Needed" text
    private IInteractable currentInteractable; // Track the currently targeted interactable
    [SerializeField] private string[] tagsToIgnore;

    [Header("Pickup Settings")]
    [SerializeField] public Transform holdPoint; // Assign in Inspector (child of the camera)
    [SerializeField] public float throwForce = 10f;
    private GameObject heldObject;

    // Timer for "Keys Needed" visibility
    private float keysNeededTimer = 0f;
    private const float KeysNeededDisplayTime = 2f; // Time in seconds to show "Keys Needed"

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = defaultHeight;
        currentStamina = maxStamina;

        audioSource.enabled = false;

        // Initialize UI elements
        if (keysNeededText != null)
        {
            keysNeededText.gameObject.SetActive(false); // Hide "Keys Needed" text by default
            keysNeededText.text = "Key Required"; // Set default text
        }

        // Ignore collisions between the player and tagged objects
        foreach (GameObject objectToIgnore in GameObject.FindGameObjectsWithTag("NoCollide"))
        {
            foreach (Collider collider in objectToIgnore.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(GetComponent<Collider>(), collider, true);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
        UpdateStaminaBar();
        HandleStaminaBarVisibility();
        HandleFootstepSounds();
        if (heldObject == null) CheckForInteractable();
        HandleInteraction();
        UpdateKeysNeededVisibility(); 
    }

    public void Pickup(GameObject obj)
    {
        if (heldObject == null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // Disable physics

            obj.transform.SetParent(holdPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            heldObject = obj;
            currentInteractable = null;
            interactionText.gameObject.SetActive(false);
            Debug.Log("PICKED UP OBJECT");
        }
    }

    public void Drop()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics
                rb.AddForce(holdPoint.forward * throwForce, ForceMode.Impulse); // Throw
            }

            heldObject.transform.SetParent(null); // Unparent
            heldObject = null;
            Debug.Log("DROPPED OBJECT");
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        isMoving = moveX != 0 || moveZ != 0;
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !isCrouching;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            characterController.height = Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * 10);
        }
        else
        {
            isCrouching = false;
            characterController.height = Mathf.Lerp(characterController.height, defaultHeight, Time.deltaTime * 10);
        }

        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 ? runSpeed : walkSpeed);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        moveDirection.x = move.x * speed;
        moveDirection.z = move.z * speed;

        if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void HandleStamina()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && characterController.velocity.magnitude > 0.1f)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    void UpdateStaminaBar()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }

    void HandleStaminaBarVisibility()
    {
        if (staminaBar == null) return;

        if (currentStamina >= maxStamina)
        {
            staminaFullTimer += Time.deltaTime;
            if (staminaFullTimer >= StaminaFullDelay)
            {
                staminaBar.gameObject.SetActive(false);
                staminaBarBackground.gameObject.SetActive(false);
            }
        }
        else
        {
            staminaFullTimer = 0f;
            staminaBar.gameObject.SetActive(true);
            staminaBarBackground.gameObject.SetActive(true);
        }
    }

    void HandleFootstepSounds()
    {
        if (isMoving)
        {
            audioSource.enabled = true;
            AudioClip targetClip = isSprinting ? runSound : walkSound;

            if (!audioSource.isPlaying || audioSource.clip != targetClip)
            {
                audioSource.clip = targetClip;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            // if (hit.collider.isTrigger) continue;

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.gameObject.SetActive(true);

                for (int j = i; j < hits.Length; j++)
                {
                    if (!((interactableLayer.value & (1 << hit.collider.gameObject.layer)) > 0))
                    {
                        break;
                    }

                    if (hits[j].collider.CompareTag("Pill"))
                    {
                        currentInteractable = hits[j].collider.GetComponent<IInteractable>();
                        break;
                    }
                }
                
                return;
            }
            else
            {
                if (hit.collider.CompareTag("PlayerClip")) continue;
                currentInteractable = null;
                interactionText.gameObject.SetActive(false);
                return;
            }
        }

        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }

    private void HandleInteraction()
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
                    if (!KeyInventory.Instance.HasKey())
                    {
                        ShowKeysNeededText();
                        KeysAudioSource.clip = OpenDoorFailedSound;
                        KeysAudioSource.Play();
                    }
                    else
                    {
                        currentInteractable.Interact();
                        KeysAudioSource.clip = UnlockDoorSound;
                        KeysAudioSource.Play();
                    }
                }
                else
                {
                    currentInteractable.Interact(); 
                }
                
            }
        }
    }

    private void ShowKeysNeededText()
    {
        if (keysNeededText != null)
        {
            keysNeededText.gameObject.SetActive(true);
            keysNeededTimer = KeysNeededDisplayTime; // Reset timer
        }
    }

    private void UpdateKeysNeededVisibility()
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