using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenShelfScript : MonoBehaviour
{
    [SerializeField] private Animator kitchenShelfAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip kitchenShelfOpenSound;
    [SerializeField] private AudioClip kitchenShelfCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        kitchenShelfAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the kitchen shelf is on cooldown
        }

        if (isOpen)
        {
            // Close the kitchen shelf
            kitchenShelfAnimator.SetTrigger("CloseKitchenShelf");
            audioSource.enabled = true;
            audioSource.clip = kitchenShelfCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the kitchen shelf
            kitchenShelfAnimator.SetTrigger("OpenKitchenShelf");
            audioSource.enabled = true;
            audioSource.clip = kitchenShelfOpenSound;
            audioSource.Play();
            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
