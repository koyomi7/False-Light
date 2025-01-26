using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeScript : MonoBehaviour
{
    [SerializeField] private Animator wardrobeAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wardrobeOpenSound;
    [SerializeField] private AudioClip wardrobeCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        wardrobeAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
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

    public void Interact()
    {
        if (isOnCooldown)
        {
            return; // Exit the method if the wardrobe is on cooldown
        }

        if (isOpen)
        {
            // Close the wardrobe
            wardrobeAnimator.SetTrigger("CloseWardrobe");
            audioSource.enabled = true;
            audioSource.clip = wardrobeCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the wardrobe
            wardrobeAnimator.SetTrigger("OpenWardrobe");
            audioSource.enabled = true;
            audioSource.clip = wardrobeOpenSound;
            audioSource.Play();

            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
