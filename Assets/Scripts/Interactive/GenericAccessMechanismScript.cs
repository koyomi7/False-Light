using System.Collections;
using UnityEngine;

public class GenericAccessMechanismScript : MonoBehaviour, IInteractable
{
    [Header("State Settings")]
    [SerializeField] public states state = states.CLOSED;

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
    public bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    [SerializeField] float CooldownDuration = 1f;

    [Header("Key Settings")]
    [SerializeField] public bool requiresKey = false; 
    [SerializeField] public bool isUnlocked = false; 

    // Other variables
    private Transform player;
    public enum states { CLOSED, OPEN, PARTLY_OPEN_1, PARTLY_OPEN_2 };

    void Start()
    {
        animator = GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

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

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) isOnCooldown = false;
        }
    }

    private IEnumerator RandomInteractionCoroutine()
    {
        while (enableRandomInteractions)
        {
            float waitTime = Random.Range(minTimeBetweenRandomInteractions, maxTimeBetweenRandomInteractions);
            yield return new WaitForSeconds(waitTime);

            if (IsPlayerNearby() && Random.value < randomInteractionChance)
                RandomInteract();
        }
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= playerDetectionRadius;
    }

    private void RandomInteract()
    {
        if (isOnCooldown) return;

        float randomValue = Random.value;
        
        // if door is locked, do not open it randomly
        if (requiresKey && !isUnlocked && state.Equals(states.CLOSED)){
            Debug.Log("Door is locked, cannot open randomly");
            return;
        } 

        if ((state.Equals(states.CLOSED) && randomValue > 0.5f) || (!state.Equals(states.CLOSED) && randomValue <= 0.5f))
            Interact();
    }

    public void Interact()
    {
        if (isOnCooldown) return;

        // Check if a key is required and the door is still locked
        if (requiresKey && !isUnlocked && state == states.CLOSED)
        {
            if (!KeyInventory.Instance.HasKey())
            {
                Debug.Log("You need a key to open this!");
                return; // Exit if no key is available
            }
            KeyInventory.Instance.UseKey(); // Consume the key
            isUnlocked = true; // Mark the door as unlocked
        }

        // Proceed with state change and animation
        animator.SetTrigger(state.ToString());
        switch (state)
        {
            case states.CLOSED:
                state = states.OPEN;
                audioSource.clip = openSound;
                break;
            case states.OPEN:
                state = states.CLOSED;
                audioSource.clip = closeSound;
                break;
            case states.PARTLY_OPEN_1:
                state = states.CLOSED;
                audioSource.clip = closeSound;
                break;
            case states.PARTLY_OPEN_2:
                state = states.CLOSED;
                audioSource.clip = closeSound;
                break;
        }
        audioSource.Play();
        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}