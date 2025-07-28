using System.Collections;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch Settings")]
    const float standingHeight = 1.5f;
    [SerializeField] float crouchHeight = 0.75f;
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("References")]
    [SerializeField] private Transform playerObj;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private CharacterController controller;

    // Original values
    private float originalControllerHeight;
    private float originalControllerCenterY;
    float originalPlayerPositionY;
    private float originalPlayerScaleY;
    private float originalCameraPosY;

    // Target values
    private float targetControllerHeight;
    private float targetControllerCenterY;
    float targetPlayerPositionY;
    private float targetPlayerScaleY;
    private float targetCameraPosY;

    // Current state
    private bool isCrouching = false;
    private Coroutine crouchCoroutine;
    float crouchHeightRatio;

    void Start()
    {
        // Ratio
        crouchHeightRatio = crouchHeight / standingHeight;
        // Store original values
        originalControllerHeight = controller.height;
        originalControllerCenterY = controller.center.y;
        originalPlayerPositionY = playerObj.localPosition.y;
        originalPlayerScaleY = playerObj.localScale.y;
        originalCameraPosY = cameraPos.localPosition.y;

        // Calculate target values (half height)
        targetControllerHeight = originalControllerHeight * crouchHeightRatio;
        Debug.Log(originalControllerCenterY);
        Debug.Log(targetControllerHeight);
        Debug.Log(originalPlayerScaleY);
        targetControllerCenterY = originalControllerCenterY - (originalPlayerScaleY - targetControllerHeight * 0.5f);
        targetPlayerPositionY = originalPlayerPositionY - (originalPlayerScaleY - originalPlayerScaleY * crouchHeightRatio);
        targetPlayerScaleY = originalPlayerScaleY * crouchHeightRatio;
        targetCameraPosY = originalCameraPosY - crouchHeight;
    }

    void Update()
    {
        HandleCrouching();
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Vector3 castOrigin = transform.position - Vector3.up * (targetPlayerScaleY * crouchHeightRatio);
        float castDistance = originalControllerHeight - targetControllerHeight;

        // Draw the cast origin sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(castOrigin, controller.radius);

        // Draw the cast direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(castOrigin, castOrigin + Vector3.up * castDistance);

        // Draw the end position sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(castOrigin + Vector3.up * castDistance, controller.radius);

        // Perform the actual cast for visualization
        bool canStand = !Physics.SphereCast(castOrigin, controller.radius, Vector3.up, out RaycastHit hit, castDistance);
        
        // Change color based on result
        Gizmos.color = canStand ? Color.green : Color.red;
        Gizmos.DrawWireSphere(castOrigin, controller.radius * 1.1f); // Slightly larger sphere to show result
    }

    bool CanStandUp()
    {
        // Casts a sphere from the player's crouched position to the player's standing position to check for obstacles
        Vector3 castOrigin = transform.position - Vector3.up * (targetPlayerScaleY * 0.5f);
        float castDistance = originalControllerHeight - targetControllerHeight;

        return !Physics.SphereCast(castOrigin, controller.radius, Vector3.up, out RaycastHit hit, castDistance);
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

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * crouchSpeed;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime);

            // Interpolate character controller
            controller.height = Mathf.Lerp(startControllerHeight, endControllerHeight, t);
            Vector3 newCenter = controller.center;
            newCenter.y = Mathf.Lerp(startControllerCenterY, endControllerCenterY, t);
            controller.center = newCenter;

            // Interpolate player object position
            Vector3 newPosition = playerObj.localPosition;
            newPosition.y = Mathf.Lerp(startPlayerPositionY, endPlayerPositionY, t);
            playerObj.localPosition = newPosition;

            // Interpolate player object scale
            Vector3 newScale = playerObj.localScale;
            newScale.y = Mathf.Lerp(startPlayerScaleY, endPlayerScaleY, t);
            playerObj.localScale = newScale;

            // Interpolate camera position
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

    // Public methods for external access
    public bool IsCrouching => isCrouching;
}
