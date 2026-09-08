using System.Collections;
using UnityEngine;

public class GenericAccessMechanism : MonoBehaviour, IInteractable
{
    [Header("State Settings")]
    [SerializeField] public states state = states.CLOSED;
    [SerializeField] GameObject someObject = null;

    [Header("Audio Settings")]
    [SerializeField] AudioClip closeSound;
    [SerializeField] public AudioClip openSound;
    [SerializeField] AudioClip readySound;

    [Header("Random Interaction Settings")]
    [SerializeField] bool enableRandomInteractions = true;
    [SerializeField] float minTimeBetweenRandomInteractions = 60f;
    [SerializeField] float maxTimeBetweenRandomInteractions = 120f;
    [SerializeField] float randomInteractionChance = 0.3f;

    [Header("Key Settings")]
    [SerializeField] public bool isLocked = false; // Requires key

    // Interaction cooldown variables
    [HideInInspector] public bool isOnCooldown = false;
    [HideInInspector] public float cooldownDuration = 0f; // 0: Animation clip length
    [HideInInspector] float cooldownTimer = 0f; // Timer from animation clip length OR cooldownDuration -> 0

    // Other variables
    public enum states { DEFAULT, CLOSED, OPEN, READY };
    Animator animator;
    AudioSource audioSource;
    static readonly int ToggleHash = Animator.StringToHash("Toggle");

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(state.ToString());
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = true;

        if (enableRandomInteractions)
            StartCoroutine(RandomInteractionCoroutine());
    }

    IEnumerator CooldownCoroutine(float waitTime)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(waitTime);
        isOnCooldown = false;
    }
    
    IEnumerator RandomInteractionCoroutine()
    {
        while (enableRandomInteractions)
        {
            float waitTime = Random.Range(minTimeBetweenRandomInteractions, maxTimeBetweenRandomInteractions);
            yield return new WaitForSeconds(waitTime);

            if (Random.value < randomInteractionChance)
                RandomInteract();
        }
    }
    
    void RandomInteract()
    {
        if (isOnCooldown) return;

        float randomValue = Random.value;

        // if door is locked, do not open it randomly
        if (isLocked && state == states.CLOSED)
        {
            Debug.Log("Door is locked, cannot open randomly");
            return;
        }

        if ((state.Equals(states.CLOSED) && randomValue > 0.5f) || (!state.Equals(states.CLOSED) && randomValue <= 0.5f))
            Toggle();
    }
    
    public void Interact()
    {
        if (isOnCooldown) return;

        // Check if a key is required and the door is still locked
        if (isLocked && state == states.CLOSED)
        {
            if (!KeyInventory.Instance.HasKey())
            {
                Debug.Log("You need a key to open this!");
                return; // Exit if no key is available
            }
            KeyInventory.Instance.UseKey(); // Consume the key
            isLocked = false; // Mark the door as unlocked
        }

        // Proceed with state change and animation
        Toggle();
        animator.Update(0f); // Forces the Animator to process the trigger immediately so we can read state info in this same frame
        cooldownTimer = (cooldownDuration == 0) ? animator.GetCurrentAnimatorStateInfo(0).length : cooldownDuration;
        StartCoroutine(CooldownCoroutine(cooldownTimer));
    }

    public void Toggle(states force = states.DEFAULT, bool playAudio = true)
    {
        if (force == states.CLOSED && state == states.CLOSED) return;
        if (force == states.OPEN && state == states.OPEN) return;

        animator.SetTrigger(ToggleHash);
        switch (state)
        {
            case states.CLOSED:
                state = states.OPEN;
                audioSource.clip = openSound;
                if (someObject != null) someObject.SetActive(true);
                break;
            case states.OPEN:
                state = states.CLOSED;
                audioSource.clip = closeSound;
                if (someObject != null) someObject.SetActive(false);
                break;
            case states.READY:
                audioSource.clip = readySound;
                break;
        }
        if (playAudio) audioSource.Play();
    }
}
