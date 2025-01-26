using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfScript : MonoBehaviour
{
    [SerializeField] private Animator shelfAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shelfOpenSound;
    [SerializeField] private AudioClip shelfCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        shelfAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the shelf is on cooldown
        }

        if (isOpen)
        {
            // Close the shelf
            shelfAnimator.SetTrigger("CloseShelf");
            audioSource.enabled = true;
            audioSource.clip = shelfCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the shelf
            shelfAnimator.SetTrigger("OpenShelf");
            audioSource.enabled = true;
            audioSource.clip = shelfOpenSound;
            audioSource.Play();
            isOpen = true;
        }
    }
}
