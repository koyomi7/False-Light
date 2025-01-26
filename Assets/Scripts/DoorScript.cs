using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip DoorOpenSound;
    [SerializeField] public AudioClip DoorCloseSound;

    private bool isOnCooldown = false; 
    private float cooldownTimer = 0f; 
    private const float CooldownDuration = 1f; 

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
    }

    void Update()
    {
        // Cooldown time to avoid spams
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false; // End the cooldown
            }
        }
    }

    public void Interact()
    {
        if (isOnCooldown)
        {
            return; // Exit the method if the door is on cooldown
        }

        if (isOpen)
        {
            // Close the door
            doorAnimator.SetTrigger("CloseDoor");
            audioSource.enabled = true;
            audioSource.clip = DoorCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the door
            doorAnimator.SetTrigger("OpenDoor");
            audioSource.enabled = true;
            audioSource.clip = DoorOpenSound;
            audioSource.Play();
            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}