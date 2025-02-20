using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerDoorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator drawerDoorAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawerDoorOpenSound;
    [SerializeField] private AudioClip drawerDoorCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        drawerDoorAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the drawer door is on cooldown
        }

        if (isOpen)
        {
            // Close the drawer door
            drawerDoorAnimator.SetTrigger("CloseDrawerDoor");
            audioSource.enabled = true;
            audioSource.clip = drawerDoorCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the drawer door
            drawerDoorAnimator.SetTrigger("OpenDrawerDoor");
            audioSource.enabled = true;
            audioSource.clip = drawerDoorOpenSound;
            audioSource.Play();
            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
