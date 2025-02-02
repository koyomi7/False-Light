using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WDrawerScript : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator wDrawerAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wDrawerOpenSound;
    [SerializeField] private AudioClip wDrawerCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        wDrawerAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the wardrobe drawer is on cooldown
        }

        if (isOpen)
        {
            // Close the wardrobe drawer
            wDrawerAnimator.SetTrigger("CloseWDrawer");
            audioSource.enabled = true;
            audioSource.clip = wDrawerCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the wardrobe drawer
            wDrawerAnimator.SetTrigger("OpenWDrawer");
            audioSource.enabled = true;
            audioSource.clip = wDrawerOpenSound;
            audioSource.Play();

            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
