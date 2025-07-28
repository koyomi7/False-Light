using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float crouchSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [Header("Stamina")]
    [SerializeField] float maxStamina;
    [SerializeField] float staminaDepletionRate;
    [SerializeField] float staminaRegenRate;

    [Header("Sounds")]
    [SerializeField] AudioSource footstepsAudioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip sprintSound;

    [Header("Other")]
    [SerializeField] Transform orientation;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Image staminaBar;
    [SerializeField] Image staminaBarBackground;

    const float gravity = -9.81f;
    const float defaultHeight = 1.5f;
    const float crouchHeight = 0.3f;
    const float heightSpeed = 10f;
    const float staminaFullDelay = 2f; // Time in seconds before hiding the stamina bar
    const float staminaEmptyDelay = 2f; // Time in seconds before being able to sprint on empty stamina
    float horizontalInput;
    float verticalInput;
    float moveSpeed;
    float stamina;
    float staminaFullTimer;
    float staminaEmptyTimer;
    bool isGrounded;
    bool isMoving;
    bool isCrouching;
    bool isSprinting;
    Vector3 moveDirection;
    Vector3 velocity;
    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
        staminaFullTimer = staminaFullDelay;
        staminaEmptyTimer = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
        // HandleCrouching();
        HandleSprinting();
        HandleGravity();
        HandleMovement();
        HandleFootstepSounds();
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isMoving = horizontalInput != 0 || verticalInput != 0;
    }

    void HandleCrouching()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * heightSpeed);
        }
        else
        {
            isCrouching = false;
            controller.height = Mathf.Lerp(controller.height, defaultHeight, Time.deltaTime * heightSpeed);
        }
    }

    void HandleSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && stamina > 0 && staminaEmptyTimer == 0)
        {
            isSprinting = true;
            stamina -= staminaDepletionRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        else
        {
            isSprinting = false;

            // Delays sprinting if stamina is used up completely
            if (stamina <= 0 || staminaEmptyTimer > 0)
            {
                staminaEmptyTimer += Time.deltaTime;
                if (staminaEmptyTimer >= staminaEmptyDelay) staminaEmptyTimer = 0f;
            }

            // Regenerates stamina
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        // Stamina Bar UI
        if (staminaBar != null)
        {
            staminaBar.fillAmount = stamina / maxStamina;
            if (stamina >= maxStamina)
            {
                staminaFullTimer += Time.deltaTime;
                if (staminaFullTimer >= staminaFullDelay)
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
    }

    void HandleGravity()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // Small downward force to stick to ground
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleMovement()
    {
        // Calculates movement direction relative to orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Ensures horizontal-only movement
        moveDirection.y = 0;

        // Normalizes movement to prevent faster diagonal movement
        if (moveDirection.magnitude > 0) moveDirection.Normalize();

        // Gets correct movement speed (crouching, walking, or sprinting)
        moveSpeed = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;

        // Applies horizontal movement
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Applies vertical velocity (gravity)
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootstepSounds()
    {
        if (isMoving)
        {
            AudioClip targetClip = isSprinting ? sprintSound : walkSound;

            if (!footstepsAudioSource.isPlaying || footstepsAudioSource.clip != targetClip)
            {
                footstepsAudioSource.clip = targetClip;
                footstepsAudioSource.Play();
            }
        }
        else
        {
            if (footstepsAudioSource.isPlaying)
            {
                footstepsAudioSource.Stop();
            }
        }
    }
}
