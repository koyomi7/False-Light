using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float crouchSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;

    [Header("Crouching")]
    [SerializeField] KeyCode crouchKey;
    [SerializeField, Range(minCrouchHeight, standingHeight)] float crouchHeight;
    [SerializeField] float crouchTransitionSpeed;

    [Header("Stamina")]
    [SerializeField] float maxStamina;
    [SerializeField] float staminaDepletionRate;
    [SerializeField] float staminaRegenRate;

    [Header("Sounds")]
    [SerializeField] AudioSource footstepsAudioSource;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip sprintSound;

    [Header("References")]
    [SerializeField] Transform playerObj;
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraPos;
    [SerializeField] Image staminaBar;
    [SerializeField] Image staminaBarBackground;

    const float controllerRadius = 0.1875f;
    const float standingHeight = 1.5f;
    const float minCrouchHeight = controllerRadius * 2;
    const float gravity = -9.81f;
    const float staminaFullDelay = 2f; // Time in seconds before hiding the stamina bar
    const float staminaEmptyDelay = 2f; // Time in seconds before being able to sprint on empty stamina

    float horizontalInput;
    float verticalInput;

    // Original values for crouching
    float originalControllerHeight;
    float originalControllerCenterY;
    float originalPlayerPositionY;
    float originalPlayerScaleY;
    float originalCameraPosY;

    // Target values for crouching
    float targetControllerHeight;
    float targetControllerCenterY;
    float targetPlayerPositionY;
    float targetPlayerScaleY;
    float targetCameraPosY;
    
    float crouchHeightRatio;
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
    Coroutine crouchCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
        staminaFullTimer = staminaFullDelay;
        staminaEmptyTimer = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Standing-to-crouching Ratio
        crouchHeightRatio = crouchHeight / standingHeight;

        // Stores original values for crouching
        originalControllerHeight = controller.height;
        originalControllerCenterY = controller.center.y;
        originalPlayerPositionY = playerObj.localPosition.y;
        originalPlayerScaleY = playerObj.localScale.y;
        originalCameraPosY = cameraPos.localPosition.y;

        // Calculates target values for crouching
        targetControllerHeight = originalControllerHeight * crouchHeightRatio;
        targetControllerCenterY = originalControllerCenterY - (originalPlayerScaleY - targetControllerHeight * 0.5f);
        targetPlayerPositionY = originalPlayerPositionY - (originalPlayerScaleY - originalPlayerScaleY * crouchHeightRatio);
        targetPlayerScaleY = originalPlayerScaleY * crouchHeightRatio;
        targetCameraPosY = crouchHeight - originalCameraPosY;

        // Ignore collisions between the player and tagged objects
        foreach (GameObject objectToIgnore in GameObject.FindGameObjectsWithTag("NoCollide"))
        {
            // "NoCollide" only needs to be applied to the parent object
            foreach (Collider collider in objectToIgnore.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(GetComponent<Collider>(), collider, true);
        }
    }

    void Update()
    {
        HandleInput();
        HandleCrouching();
        HandleSprinting();
        HandleGravity();
        HandleMovement();
        HandleFootstepSounds();
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        CanStandUpDebug();
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isMoving = horizontalInput != 0 || verticalInput != 0;
    }

    void HandleCrouching()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            // Crouch -> Stand
            if (isCrouching)
            {
                // Checks if there's enough space to stand up
                if (CanStandUp())
                {
                    if (crouchCoroutine != null)
                        StopCoroutine(crouchCoroutine);

                    crouchCoroutine = StartCoroutine(SmoothCrouch(false));
                }
            }
            // Stand -> Crouch
            else
            {
                if (crouchCoroutine != null)
                    StopCoroutine(crouchCoroutine);

                crouchCoroutine = StartCoroutine(SmoothCrouch(true));
            }
        }
    }

    bool CanStandUp()
    {
        // Slightly smaller radius than hitbox (capsule collider/player controller) to avoid imprecision glitches when attempting to stand
        float reducedRadius = controllerRadius * 0.95f;

        // Casts a sphere from the player's crouched position to the player's standing position to check for obstacles
        Vector3 castOrigin = transform.position + Vector3.up * (-standingHeight * 0.5f + targetControllerHeight - reducedRadius);
        float castDistance = originalControllerHeight - targetControllerHeight;

        // Excludes the layer of interactable objects
        int layerMask = ~(1 << LayerMask.NameToLayer("Interactable"));

        return !Physics.SphereCast(castOrigin, reducedRadius, Vector3.up, out RaycastHit hit, castDistance, layerMask);
    }

    void CanStandUpDebug()
    {
        // Slightly smaller radius than hitbox (capsule collider/player controller) to avoid imprecision glitches when attempting to stand
        float reducedRadius = controllerRadius * 0.95f;

        // Casts a sphere from the player's crouched position to the player's standing position to check for obstacles
        Vector3 castOrigin = transform.position + Vector3.up * (-standingHeight * 0.5f + targetControllerHeight - reducedRadius);
        float castDistance = originalControllerHeight - targetControllerHeight;

        // Excludes the layer of interactable objects
        int layerMask = ~(1 << LayerMask.NameToLayer("Interactable"));

        // Draws the cast origin sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(castOrigin, reducedRadius);

        // Draws the cast direction sphere and end point
        Vector3 castEnd = castOrigin + Vector3.up * castDistance;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(castOrigin, castEnd);
        Gizmos.DrawWireSphere(castEnd, reducedRadius);

        // Perform the actual sphere cast for visualization
        if (Physics.SphereCast(castOrigin, reducedRadius, Vector3.up, out RaycastHit hit, castDistance, layerMask))
        {
            // Draws the hit point and surface normal
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.point, 0.1f);
            Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.5f);
            
            // Draws the sphere at the hit point
            Gizmos.DrawWireSphere(hit.point, reducedRadius);
        }
        else
        {
            // Draws successful path in green if no obstruction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(castOrigin, castEnd);
        }
    }

    IEnumerator SmoothCrouch(bool crouch)
    {
        isCrouching = crouch;

        float startControllerHeight = controller.height;
        float startControllerCenterY = controller.center.y;
        float startPlayerPositionY = playerObj.localPosition.y;
        float startPlayerScaleY = playerObj.localScale.y;
        float startCameraPosY = cameraPos.localPosition.y;

        float endControllerHeight = crouch ? targetControllerHeight : originalControllerHeight;
        float endControllerCenterY = crouch ? targetControllerCenterY : originalControllerCenterY;
        float endPlayerPositionY = crouch ? targetPlayerPositionY : originalPlayerPositionY;
        float endPlayerScaleY = crouch ? targetPlayerScaleY : originalPlayerScaleY;
        float endCameraPosY = crouch ? targetCameraPosY : originalCameraPosY;

        float elapsedTime = 0f;

        // Interpolates values
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * crouchTransitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime);

            // Character controller
            controller.height = Mathf.Lerp(startControllerHeight, endControllerHeight, t);
            Vector3 newCenter = controller.center;
            newCenter.y = Mathf.Lerp(startControllerCenterY, endControllerCenterY, t);
            controller.center = newCenter;

            // Player object position
            Vector3 newPosition = playerObj.localPosition;
            newPosition.y = Mathf.Lerp(startPlayerPositionY, endPlayerPositionY, t);
            playerObj.localPosition = newPosition;

            // Player object scale
            Vector3 newScale = playerObj.localScale;
            newScale.y = Mathf.Lerp(startPlayerScaleY, endPlayerScaleY, t);
            playerObj.localScale = newScale;

            // Camera position
            Vector3 newCameraPos = cameraPos.localPosition;
            newCameraPos.y = Mathf.Lerp(startCameraPosY, endCameraPosY, t);
            cameraPos.localPosition = newCameraPos;

            yield return null;
        }

        // Ensure final values are set exactly
        controller.height = endControllerHeight;
        Vector3 finalCenter = controller.center;
        finalCenter.y = endControllerCenterY;
        controller.center = finalCenter;

        Vector3 finalPosition = playerObj.localPosition;
        finalPosition.y = endPlayerPositionY;
        playerObj.localPosition = finalPosition;

        Vector3 finalScale = playerObj.localScale;
        finalScale.y = endPlayerScaleY;
        playerObj.localScale = finalScale;

        Vector3 finalCameraPos = cameraPos.localPosition;
        finalCameraPos.y = endCameraPosY;
        cameraPos.localPosition = finalCameraPos;
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
