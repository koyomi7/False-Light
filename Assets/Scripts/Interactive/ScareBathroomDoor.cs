using UnityEngine;
using System.Collections;

public class ScareBathroomDoorScript : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip DoorOpenSound;
    [SerializeField] private AudioClip DoorCloseSound;
    [SerializeField] private AudioClip FootstepsBehindDoor;
    [SerializeField] private AudioClip DoorSlamSound; 
    [SerializeField] private GameObject airWall;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
        audioSource.enabled = false;
        airWall.SetActive(false);
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
        // Play footsteps
        PlaySound(FootstepsBehindDoor);

        // Wait for footsteps to finish 
        yield return new WaitForSeconds(FootstepsBehindDoor.length);

        // Close the door
        CloseDoor();
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