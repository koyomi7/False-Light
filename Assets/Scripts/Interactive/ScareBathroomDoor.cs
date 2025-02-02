using UnityEngine;
using System.Collections;

public class ScareBathroomDoorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip DoorOpenSound;
    [SerializeField] private AudioClip DoorCloseSound;
    [SerializeField] private AudioClip FootstepsBehindDoor;
    [SerializeField] private AudioClip DoorSlamSound; 
    [SerializeField] private GameObject airWall;
    [SerializeField] private GameObject Ghost;
    [SerializeField] private Transform doorPos;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
        audioSource.enabled = false;
        airWall.SetActive(false);
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
        PlaySound(DoorOpenSound);
        isOpen = true;
    }

    public void CloseDoor()
    {
        doorAnimator.SetTrigger("CloseDoor");
        PlaySound(DoorCloseSound);
        isOpen = false;
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.enabled = true;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayScareEvent()
    {
        if (isOpen)
        {
            Debug.Log("Playing footsteps and closing door");
            StartCoroutine(FootstepsAndCloseDoorSequence());
        }
        else
        {
            Debug.Log("Playing scary sound and door slam");
            StartCoroutine(ScaryAndSlamSequence());
        }

        // Disable the scare trigger after activation
        if (scareTrigger != null)
        {
            scareTrigger.SetActive(false);
            Debug.Log("Scare trigger disabled");
        }
    }

    private IEnumerator FootstepsAndCloseDoorSequence()
    {
        airWall.SetActive(true);
        
        // Enable and setup ghost
        Ghost.SetActive(true);
        ghostAnimator.Play("GoofyRun");
        
        // Play footsteps from ghost
        ghostAudioSource.enabled = true;
        ghostAudioSource.clip = FootstepsBehindDoor;
        ghostAudioSource.Play();

        float runDuration = 1.5f; 
        float elapsedTime = 0f;
        Vector3 startPos = Ghost.transform.position;
        Vector3 targetPos = new Vector3(doorPos.position.x, Ghost.transform.position.y, doorPos.position.z);

        // Move ghost to door
        while (elapsedTime < runDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / runDuration;

            // Calculate direction and rotation
            Vector3 directionToDoor = (targetPos - Ghost.transform.position).normalized;
            directionToDoor.y = 0;

            if (directionToDoor != Vector3.zero)
            {
                Ghost.transform.forward = directionToDoor;
            }

            // Move ghost
            Ghost.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // Close door
        CloseDoor();

        // Play door slam sound
        yield return new WaitForSeconds(0.5f); // Wait for door animation to start
        PlaySound(DoorSlamSound);

        // Wait extra second before destroying ghost
        yield return new WaitForSeconds(1f);
        
        // Cleanup
        Destroy(Ghost);
        airWall.SetActive(false);
    }

    private IEnumerator ScaryAndSlamSequence()
    {
        airWall.SetActive(true);
        // Play the scary sound
        PlaySound(FootstepsBehindDoor);

        // Wait for the scary sound to finish 
        yield return new WaitForSeconds(FootstepsBehindDoor.length);

        // Play the door slam sound
        PlaySound(DoorSlamSound);
        airWall.SetActive(false);
    }
}