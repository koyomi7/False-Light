using UnityEngine;
using System.Collections;

public class ScareDoorScript : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip DoorOpenSound;
    [SerializeField] private AudioClip DoorCloseSound;
    [SerializeField] private AudioClip FootstepsBehindDoor; 
    [SerializeField] private GameObject Ghost;
    [SerializeField] private Transform player; // Reference to player for ghost to move towards

    private bool isOnCooldown = false; 
    private float cooldownTimer = 0f; 
    private const float CooldownDuration = 1f; 

    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
        audioSource.enabled = false;
        Ghost.SetActive(false);
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
            }
        }
    }

    public void Interact()
    {
        if (isOnCooldown) return;

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
            StartCoroutine(RunningGhostSequence());
        }
        else
        {
            Debug.Log("STARE AT YO FACE");
            StartCoroutine(HangingGhostSequence());
        }

        if (scareTrigger != null)
        {
            scareTrigger.SetActive(false); 
            Debug.Log("Scare trigger disabled");
        }
    }

private IEnumerator RunningGhostSequence()
    {
        // Setup ghost
        Ghost.SetActive(true);
        ghostAnimator.Play("FastCrawl");
        
        // Play footstep sound
        ghostAudioSource.enabled = true;
        ghostAudioSource.clip = FootstepsBehindDoor;
        audioSource.Play();

        float elapsedTime = 0f;
        Vector3 startPos = Ghost.transform.position;
        // Only use X and Z from player position, keep ghost's Y position
        Vector3 targetPos = new Vector3(player.position.x, Ghost.transform.position.y, player.position.z);
        
        while (elapsedTime < 1.2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.2f;
            
            // Calculate direction to player but only on X and Z axes
            Vector3 directionToPlayer = (targetPos - Ghost.transform.position).normalized;
            directionToPlayer.y = 0; // Keep y rotation level
            
            // Make ghost face player on ground plane
            if(directionToPlayer != Vector3.zero) // Prevent error when vectors are identical
            {
                Ghost.transform.forward = directionToPlayer;
            }
            
            // Move ghost
            Ghost.transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            yield return null;
        }

        // Destroy ghost after reaching player
        Destroy(Ghost);
    }

    private IEnumerator HangingGhostSequence()
    {
        // Setup ghost
        Ghost.SetActive(true);
        Ghost.transform.Rotate(0, 0, 180);
        Ghost.transform.position = new Vector3(Ghost.transform.position.x, 5.5f, Ghost.transform.position.z);
        ghostAnimator.Play("Hanging");
        OpenDoor();

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        // Destroy ghost
        Destroy(Ghost);
    }
}