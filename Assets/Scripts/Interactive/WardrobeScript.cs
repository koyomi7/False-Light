using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeScript : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioClip wardrobeOpenSound;
    [SerializeField] private AudioClip wardrobeCloseSound;
    [HideInInspector] private Animator wardrobeAnimator;
    [HideInInspector] private AudioSource audioSource;
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
        wardrobeAnimator = GetComponent<Animator>();
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

    public void Interact(){
        if (isOnCooldown) return; // Exit the method if the wardrobe is on cooldown
        

        if (isOpen)
        {
            CloseWardrobe();
        }
        else
        {
            OpenWardrobe();
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }

    public void OpenWardrobe()
    {
        wardrobeAnimator.SetTrigger("OpenWardrobe");
        audioSource.enabled = true;
        audioSource.clip = wardrobeOpenSound;
        audioSource.Play();
        isOpen = true;
    }

    public void CloseWardrobe()
    {
        wardrobeAnimator.SetTrigger("CloseWardrobe");
        audioSource.enabled = true;
        audioSource.clip = wardrobeCloseSound;
        audioSource.Play();
        isOpen = false;
    }
}
