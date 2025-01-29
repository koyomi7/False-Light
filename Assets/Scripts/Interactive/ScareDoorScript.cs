using UnityEngine;

public class ScareDoorScript : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip DoorOpenSound;
    [SerializeField] private AudioClip DoorCloseSound;
    [SerializeField] private AudioClip FootstepsBehindDoor; 

    private bool isOnCooldown = false; 
    private float cooldownTimer = 0f; 
    private const float CooldownDuration = 1f; 

    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
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
        if (isOnCooldown) return; // Exit if cooldown is active

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }

    public void OpenDoor()
    {
        doorAnimator.SetTrigger("OpenDoor");
        audioSource.enabled = true;
        audioSource.clip = DoorOpenSound;
        audioSource.Play();
        isOpen = true;
    }

    public void CloseDoor()
    {
        doorAnimator.SetTrigger("CloseDoor");
        audioSource.enabled = true;
        audioSource.clip = DoorCloseSound;
        audioSource.Play();
        isOpen = false;
    }

    public void PlayScareEvent()
    {
        if (isOpen)
        {
            Debug.Log("RUNNING TO YO FACE");
            // If the door is open, play scary footsteps sound
            audioSource.enabled = true;
            audioSource.clip = FootstepsBehindDoor;
            audioSource.Play();
        }
        else
        {
            // If the door is closed, open it
            Debug.Log("STARE AT YO FACE");
            OpenDoor();
        }

        // Disable the scare trigger after activation
        if (scareTrigger != null)
        {
            scareTrigger.SetActive(false); 
            Debug.Log("Scare trigger disabled");
        }
    }
}
