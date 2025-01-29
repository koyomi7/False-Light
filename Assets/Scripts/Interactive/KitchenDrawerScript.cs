using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenDrawerScript : MonoBehaviour
{
    [SerializeField] private Animator kitchenDrawerAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip kitchenDrawerOpenSound;
    [SerializeField] private AudioClip kitchenDrawerCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        kitchenDrawerAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the kitchen drawer is on cooldown
        }

        if (isOpen)
        {
            // Close the kitchen drawer
            kitchenDrawerAnimator.SetTrigger("CloseKitchenDrawer");
            audioSource.enabled = true;
            audioSource.clip = kitchenDrawerCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the kitchen drawer
            kitchenDrawerAnimator.SetTrigger("OpenKitchenDrawer");
            audioSource.enabled = true;
            audioSource.clip = kitchenDrawerOpenSound;
            audioSource.Play();

            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
