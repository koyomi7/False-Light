using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;
    public float smoothness = 0.5f;

    private Vector3 targetPosition;
    private float currentRotationX;
    private float currentRotationY;

    void Start()
    {
        targetPosition = transform.position;
        currentRotationX = transform.eulerAngles.y;
        currentRotationY = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Forward/Backward movement
        if (Input.GetKey(KeyCode.W))
            targetPosition += transform.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            targetPosition -= transform.forward * moveSpeed * Time.deltaTime;

        // Left/Right movement
        if (Input.GetKey(KeyCode.A))
            targetPosition -= transform.right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            targetPosition += transform.right * moveSpeed * Time.deltaTime;

        // Up/Down movement
        if (Input.GetKey(KeyCode.E))
            targetPosition += transform.up * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            targetPosition -= transform.up * moveSpeed * Time.deltaTime;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness);
    }

    void HandleRotation()
    {
        // Mouse rotation when right mouse button is held
        if (Input.GetMouseButton(1))
        {
            currentRotationX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentRotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            
            // Clamp vertical rotation to prevent camera flipping
            currentRotationY = Mathf.Clamp(currentRotationY, -90f, 90f);
        }

        // Apply rotation
        transform.rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
    }
}
