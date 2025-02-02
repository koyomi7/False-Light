using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeDoorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator fridgeDoorAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fridgeDoorOpenSound;
    [SerializeField] private AudioClip fridgeDoorCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        fridgeDoorAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the fridge door is on cooldown
        }

        if (isOpen)
        {
            // Close the fridge door
            fridgeDoorAnimator.SetTrigger("CloseFridgeDoor");
            audioSource.enabled = true;
            audioSource.clip = fridgeDoorCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the fridge door
            fridgeDoorAnimator.SetTrigger("OpenFridgeDoor");
            audioSource.enabled = true;
            audioSource.clip = fridgeDoorOpenSound;
            audioSource.Play();

            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
