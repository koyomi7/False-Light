using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractable
{

    [SerializeField] private bool isOpen = false;
    [SerializeField] public AudioClip DoorOpenSound;
    [SerializeField] public AudioClip DoorCloseSound;
    [HideInInspector] private Animator doorAnimator;
    [HideInInspector] public AudioSource audioSource;

    // variables for random interactions
    [SerializeField] private bool enableRandomInteractions = true;
    [SerializeField] private float minTimeBetweenRandomInteractions = 60f;
    [SerializeField] private float maxTimeBetweenRandomInteractions = 120f;
    [SerializeField] private float randomInteractionChance = 0.3f; // 30% chance
    [SerializeField] private float playerDetectionRadius = 10f; // Only activate when player is nearby

    private bool isOnCooldown = false; 
    private float cooldownTimer = 0f; 
    private const float CooldownDuration = 1f; 
    private Transform player;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enableRandomInteractions)
        {
            StartCoroutine(RandomInteractionCoroutine());
        }
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

    private IEnumerator RandomInteractionCoroutine()
    {
        while (enableRandomInteractions)
        {
            // Wait for random time
            float waitTime = Random.Range(minTimeBetweenRandomInteractions, maxTimeBetweenRandomInteractions);
            yield return new WaitForSeconds(waitTime);

            // Check if player is within range and random chance is met
            if (IsPlayerNearby() && Random.value < randomInteractionChance)
            {
                RandomInteract();
            }
        }
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= playerDetectionRadius;
    }

    private void RandomInteract()
    {
        if (!isOnCooldown)
        {
            // Add some randomness to the interaction
            float randomValue = Random.value;

            if ((!isOpen && randomValue > 0.5f) || (isOpen && randomValue <= 0.5f))
            {
                Interact();
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
}