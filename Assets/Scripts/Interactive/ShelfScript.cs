using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfScript : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator shelfAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shelfOpenSound;
    [SerializeField] private AudioClip shelfCloseSound;

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
        shelfAnimator = GetComponent<Animator>();
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
                Interact();
            }
        }
    }

    private bool IsPlayerNearby()
    {
        return Vector3.Distance(player.position, transform.position) <= playerDetectionRadius;
    }

    public void Interact()
    {
        if (isOnCooldown)
        {
            return; // Exit the method if the shelf is on cooldown
        }

        if (isOpen)
        {
            closeShelf();
        }
        else
        {
            openShelf();
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }

    private void openShelf()
    {
        // Open the shelf
        shelfAnimator.SetTrigger("OpenShelf");
        audioSource.enabled = true;
        audioSource.clip = shelfOpenSound;
        audioSource.Play();

        isOpen = true;
    }

    private void closeShelf()
    {
        // Close the shelf
        shelfAnimator.SetTrigger("CloseShelf");
        audioSource.enabled = true;
        audioSource.clip = shelfCloseSound;
        audioSource.Play();

        isOpen = false;
    }
}
