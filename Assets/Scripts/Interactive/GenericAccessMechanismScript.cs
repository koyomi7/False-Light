using System.Collections;
using UnityEngine;

public class GenericAccessMechanismScript : MonoBehaviour, IInteractable {
    [Header("State Settings")]
    [SerializeField] private states state = states.CLOSED;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip openSound;

    [Header("Animation Settings")]
    [SerializeField] private AnimationClip closedClip; // if closed we want to open -> use opening clip
    [SerializeField] private AnimationClip openClip; // if open we want to close -> use closing clip
    [SerializeField] private AnimationClip partlyOpen1Clip;
    [SerializeField] private AnimationClip partlyOpen2Clip;
    [HideInInspector] private Animator animator;
    [HideInInspector] private AudioSource audioSource;

    [Header("Random Interaction Settings")]
    [SerializeField] private bool enableRandomInteractions = true;
    [SerializeField] private float minTimeBetweenRandomInteractions = 60f;
    [SerializeField] private float maxTimeBetweenRandomInteractions = 120f;
    [SerializeField] private float randomInteractionChance = 0.3f; // 30% chance
    [SerializeField] private float playerDetectionRadius = 10f; // Only activate when player is nearby
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    // Other variables
    private Transform player;
    private enum states {CLOSED, OPEN, PARTLY_OPEN_1, PARTLY_OPEN_2};

    void Start() {
        // Gets the current generic interaction runtime animator controller and creates an override controller based on it
        animator = GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        // Overrides the default animation clips
        overrideController["CLOSED"] = closedClip;
        overrideController["OPEN"] = openClip;
        overrideController["PARTLY_OPEN_1"] = partlyOpen1Clip;
        overrideController["PARTLY_OPEN_2"] = partlyOpen2Clip;

        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enableRandomInteractions)
            StartCoroutine(RandomInteractionCoroutine());
    }

    void Update() {
        // Cooldown time to avoid spams
        if (isOnCooldown) {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) isOnCooldown = false; // End the cooldown
        }
    }

    private IEnumerator RandomInteractionCoroutine() {
        while (enableRandomInteractions) {
            // Wait for random time
            float waitTime = Random.Range(minTimeBetweenRandomInteractions, maxTimeBetweenRandomInteractions);
            yield return new WaitForSeconds(waitTime);

            // Check if player is within range and random chance is met
            if (IsPlayerNearby() && Random.value < randomInteractionChance)
                RandomInteract();
        }
    }

    private bool IsPlayerNearby() {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= playerDetectionRadius;
    }

    private void RandomInteract() {
        if (isOnCooldown) return; // Exit if cooldown is active

        // Add some randomness to the interaction
        float randomValue = Random.value;

        if ((state.Equals(states.CLOSED) && randomValue > 0.5f) || (!state.Equals(states.CLOSED) && randomValue <= 0.5f))
            Interact();
    }

    public void Interact() {
        if (isOnCooldown) return; // Exit if cooldown is active

        animator.SetTrigger(state.ToString());
        switch (state) {
            case states.CLOSED: state = states.OPEN; audioSource.clip = openSound; break;
            case states.OPEN: state = states.CLOSED; audioSource.clip = closeSound; break;
            case states.PARTLY_OPEN_1: state = states.CLOSED; audioSource.clip = closeSound; break;
            case states.PARTLY_OPEN_2: state = states.CLOSED; audioSource.clip = closeSound; break;
        }
        audioSource.Play();
        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
