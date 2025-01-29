using System.Collections;
using UnityEngine;

public class DeskDrawerScript : MonoBehaviour
{
    [SerializeField] private Animator drawerAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawerOpenSound;
    [SerializeField] private AudioClip drawerCloseSound;

    // Variables for random interactions
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
        drawerAnimator = GetComponent<Animator>();
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
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
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
            
            // If drawer is closed and random value is high, open it
            // If drawer is open and random value is low, close it
            if ((!isOpen && randomValue > 0.5f) || (isOpen && randomValue <= 0.5f))
            {
                Interact();
            }
        }
    }

    public void Interact()
    {
        if (isOnCooldown) return;

        if (isOpen)
        {
            CloseDrawer();
        }
        else
        {
            OpenDrawer();
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }

    private void OpenDrawer()
    {
        drawerAnimator.SetTrigger("OpenDrawer");
        PlaySound(drawerOpenSound);
        isOpen = true;
    }

    private void CloseDrawer()
    {
        drawerAnimator.SetTrigger("CloseDrawer");
        PlaySound(drawerCloseSound);
        isOpen = false;
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.enabled = true;
        audioSource.clip = clip;
        audioSource.Play();
    }
}