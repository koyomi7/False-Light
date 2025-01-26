using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenIslandScript : MonoBehaviour
{
    [SerializeField] private Animator kitchenIslandAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip kitchenIslandOpenSound;
    [SerializeField] private AudioClip kitchenIslandCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        kitchenIslandAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the kitchen island is on cooldown
        }

        if (isOpen)
        {
            // Close the kitchen island
            kitchenIslandAnimator.SetTrigger("CloseKitchenIsland");
            audioSource.enabled = true;
            audioSource.clip = kitchenIslandCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the kitchen island
            kitchenIslandAnimator.SetTrigger("OpenKitchenIsland");
            audioSource.enabled = true;
            audioSource.clip = kitchenIslandOpenSound;
            audioSource.Play();

            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
