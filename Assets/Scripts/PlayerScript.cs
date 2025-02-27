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
    private IInteractable currentInteractable; // Track the currently targeted interactable
    [SerializeField] private string[] tagsToIgnore;

    [Header("Pickup Settings")]
    [SerializeField] public Transform holdPoint; // Assign in Inspector (child of the camera)
    [SerializeField] public float throwForce = 10f;
    private GameObject heldObject;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = defaultHeight;
        currentStamina = maxStamina;

        audioSource.enabled = false;
        
        // Ignore collisions between the player and tagged objects
        foreach (GameObject objectToIgnore in GameObject.FindGameObjectsWithTag("NoCollide")) {
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
    }

    public void Pickup(GameObject obj) {
        if (heldObject == null) {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // Disable physics

            // Parent the object to the hold point
            obj.transform.SetParent(holdPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            heldObject = obj;
            currentInteractable = null;
            interactionText.gameObject.SetActive(false);
            Debug.Log("PICKED UP OBJECT");
        }
    }

    public void Drop() {
        if (heldObject != null) {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) {
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

        // Check if stamina is full
        if (currentStamina >= maxStamina)
        {
            // Increment the timer
            staminaFullTimer += Time.deltaTime;

            // Hide the stamina bar after 2 seconds of full stamina
            if (staminaFullTimer >= StaminaFullDelay)
            {
                staminaBar.gameObject.SetActive(false);
                staminaBarBackground.gameObject.SetActive(false);
            }
        }
        else
        {
            // Reset the timer and show the stamina bar
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

    private void CheckForInteractable() {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward);

        // Sort hits by distance (closest first)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];
            // Skip triggers
            if (hit.collider.isTrigger) continue;

            // Check if the hit object is interactable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null) {
                // no obstruction
                currentInteractable = interactable;
                interactionText.gameObject.SetActive(true);

                // check remaining layers for pills
                for (int j = i; j < hits.Length; j++) {
                    // asserts that remaining layers are interactable layers
                        // i.e., a pill object within a drawer object
                    if (!((interactableLayer.value & (1 << hit.collider.gameObject.layer)) > 0)) {
                        break;
                    }

                    // returns the pill object instead
                    if (hits[j].collider.CompareTag("Pill")) {
                        currentInteractable = hits[j].collider.GetComponent<IInteractable>();
                        break;
                    }
                }
                
                return;
            }
            else {
                // The hit is a non-interactable obstruction (e.g., a wall)
                // Block interaction and exit early
                if (hit.collider.CompareTag("PlayerClip")) continue; // Can interact through player clip brush
                currentInteractable = null;
                interactionText.gameObject.SetActive(false);
                return;
            }
        }

        // No hits at all: hide the text
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }

    private void HandleInteraction() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (heldObject != null) Drop();
            else if (currentInteractable != null) currentInteractable.Interact();
        }
    }

    // void HandleInteraction() {
    //     Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //     RaycastHit hit;
    //     Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

    //     // Does not do anything for non-interactable objects
    //     if (Physics.Raycast(ray, out hit, interactionDistance, obstacleLayer)) {
    //         interactionText.gameObject.SetActive(false); // Hide interaction text
    //         return;
    //     }

    //     // Check if the ray hits an interactable object
    //     if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer)) {
    //         interactionText.gameObject.SetActive(true); // Show interaction text

    //         // Check for 'F' key press to interact
    //         if (Input.GetKeyDown(KeyCode.F)) {
    //             // Call the Interact method on the door script
    //             if (hit.collider.CompareTag("Door")){
    //                 DoorScript doors = hit.collider.GetComponent<DoorScript>();
    //                 if (doors != null) doors.Interact();
    //             }
    //             // Call the Interact method on the scare door script
    //             else if (hit.collider.CompareTag("ScareDoor")) {
    //                 ScareDoorScript scareDoor = hit.collider.GetComponent<ScareDoorScript>();
    //                 if (scareDoor != null) scareDoor.Interact();
    //             }
    //             // Call the Interact method on the scare bathroom door script
    //             else if (hit.collider.CompareTag("ScareBathroomDoor")) {
    //                 ScareBathroomDoorScript scareBathroomDoor = hit.collider.GetComponent<ScareBathroomDoorScript>();
    //                 if (scareBathroomDoor != null) scareBathroomDoor.Interact();
    //             }
    //             // Call the Interact method on the light switch script
    //             else if (hit.collider.CompareTag("LightSwitch")) {
    //                 LightSwitchScript interactable = hit.collider.GetComponent<LightSwitchScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the drawer script
    //             else if (hit.collider.CompareTag("DeskDrawer")) {
    //                 DeskDrawerScript interactable = hit.collider.GetComponent<DeskDrawerScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the shelf script
    //             else if (hit.collider.CompareTag("Shelf")) {
    //                 ShelfScript interactable = hit.collider.GetComponent<ShelfScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the sink drawer script
    //             else if (hit.collider.CompareTag("SinkDrawer")) {
    //                 SinkDrawerScript interactable = hit.collider.GetComponent<SinkDrawerScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the kitchen shelf script
    //             else if (hit.collider.CompareTag("KitchenShelf")) {
    //                 KitchenShelfScript interactable = hit.collider.GetComponent<KitchenShelfScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the kitchen island script
    //             else if (hit.collider.CompareTag("KitchenIsland")) {
    //                 KitchenIslandScript interactable = hit.collider.GetComponent<KitchenIslandScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the fridge door script
    //             else if (hit.collider.CompareTag("FridgeDoor")) {
    //                 FridgeDoorScript interactable = hit.collider.GetComponent<FridgeDoorScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the kitchen drawer script
    //             else if (hit.collider.CompareTag("KitchenDrawer")) {
    //                 KitchenDrawerScript interactable = hit.collider.GetComponent<KitchenDrawerScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the wardrobe script
    //             else if (hit.collider.CompareTag("Wardrobe")) {
    //                 WardrobeScript interactable = hit.collider.GetComponent<WardrobeScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the wardrobe drawer script
    //             else if (hit.collider.CompareTag("WDrawer")) {
    //                 WDrawerScript interactable = hit.collider.GetComponent<WDrawerScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
    //             // Call the Interact method on the pill script
    //             else if (hit.collider.CompareTag("Pill")) {
    //                 PillScript interactable = hit.collider.GetComponent<PillScript>();
    //                 if (interactable != null) interactable.Interact();
    //             }
                
    //         }
    //     }
    //     else {
    //         // Reset crosshair color if not aiming at an interactable object
    //         interactionText.gameObject.SetActive(false); // Hide interaction text
    //     }
    // }
}