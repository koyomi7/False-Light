using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour
{
    // Movement variables
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float currentWalkSpeed; // Make this serialized to see changes in the Inspector
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

    private bool canMove = true;
    private bool isMoving = false; // Track if the player is moving
    private bool isSprinting = false; // Track if the player is sprinting

    // Timer for stamina bar visibility
    private float staminaFullTimer = 0f;
    private const float StaminaFullDelay = 2f; // Time in seconds before hiding the stamina bar

    // Crosshair and interaction variables
    [SerializeField] private Image crosshair; // Reference to the crosshair UI Image
    [SerializeField] private float interactionDistance = 1f; // Max distance for interaction
    [SerializeField] private LayerMask interactableLayer; // Layer for interactable objects
    [SerializeField] private TMPro.TextMeshProUGUI interactionText; // Reference to the interaction text

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = defaultHeight;
        currentStamina = maxStamina;
        currentWalkSpeed = walkSpeed;

        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
        UpdateStaminaBar();
        HandleStaminaBarVisibility();
        HandleFootstepSounds();
        HandleInteraction();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if the player is sprinting (holding Left Shift) and has stamina
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && isMoving; // Only sprint if moving
        currentWalkSpeed = isSprinting ? runSpeed : walkSpeed;

        float curSpeedX = canMove ? currentWalkSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? currentWalkSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, defaultHeight - 1.5f, 0); // Adjust center for crouching
            currentWalkSpeed = crouchSpeed; // Set speed to crouch speed
        }
        else
        {
            characterController.height = defaultHeight;
            characterController.center = new Vector3(0, 0, 0); // Reset center when standing
            currentWalkSpeed = isSprinting ? runSpeed : walkSpeed; // Revert to walk or run speed
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Check if the player is moving
        isMoving = (Mathf.Abs(curSpeedX) > 0.1f || Mathf.Abs(curSpeedY) > 0.1f) && characterController.isGrounded;

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleStamina()
    {
        // Check if the player is sprinting (holding Left Shift) and moving
        if (isSprinting)
        {
            // Drain stamina when sprinting
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            // If stamina runs out, stop sprinting
            if (currentStamina <= 0)
            {
                isSprinting = false;
                currentWalkSpeed = walkSpeed; // Revert to walk speed when stamina runs out
            }
        }
        else
        {
            // Regenerate stamina when not sprinting
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

    void HandleInteraction() {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        // Check if the ray hits an interactable object
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer)) {
            interactionText.gameObject.SetActive(true); // Show interaction text

            // Check for 'F' key press to interact
            if (Input.GetKeyDown(KeyCode.F)) {
                // Call the Interact method on the door script
                if (hit.collider.CompareTag("Door")){
                    DoorScript doors = hit.collider.GetComponent<DoorScript>();
                    if (doors != null) doors.Interact();
                }
                // Call the Interact method on the light switch script
                else if (hit.collider.CompareTag("LightSwitch")) {
                    LightSwitchScript interactable = hit.collider.GetComponent<LightSwitchScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the drawer script
                else if (hit.collider.CompareTag("DeskDrawer")) {
                    DeskDrawerScript interactable = hit.collider.GetComponent<DeskDrawerScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the shelf script
                else if (hit.collider.CompareTag("Shelf")) {
                    ShelfScript interactable = hit.collider.GetComponent<ShelfScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the sink drawer script
                else if (hit.collider.CompareTag("SinkDrawer")) {
                    SinkDrawerScript interactable = hit.collider.GetComponent<SinkDrawerScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the kitchen shelf script
                else if (hit.collider.CompareTag("KitchenShelf")) {
                    KitchenShelfScript interactable = hit.collider.GetComponent<KitchenShelfScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the kitchen island script
                else if (hit.collider.CompareTag("KitchenIsland")) {
                    KitchenIslandScript interactable = hit.collider.GetComponent<KitchenIslandScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the fridge door script
                else if (hit.collider.CompareTag("FridgeDoor")) {
                    FridgeDoorScript interactable = hit.collider.GetComponent<FridgeDoorScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the kitchen drawer script
                else if (hit.collider.CompareTag("KitchenDrawer")) {
                    KitchenDrawerScript interactable = hit.collider.GetComponent<KitchenDrawerScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the wardrobe script
                else if (hit.collider.CompareTag("Wardrobe")) {
                    WardrobeScript interactable = hit.collider.GetComponent<WardrobeScript>();
                    if (interactable != null) interactable.Interact();
                }
                // Call the Interact method on the wardrobe drawer script
                else if (hit.collider.CompareTag("WDrawer")) {
                    WDrawerScript interactable = hit.collider.GetComponent<WDrawerScript>();
                    if (interactable != null) interactable.Interact();
                }
                
            }
        }
        else {
            // Reset crosshair color if not aiming at an interactable object
            interactionText.gameObject.SetActive(false); // Hide interaction text
        }
    }
}