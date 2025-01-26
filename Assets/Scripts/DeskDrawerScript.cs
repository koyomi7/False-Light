using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskDrawerScript : MonoBehaviour
{
    [SerializeField] private Animator drawerAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawerOpenSound;
    [SerializeField] private AudioClip drawerCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        drawerAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the drawer is on cooldown
        }

        if (isOpen)
        {
            // Close the drawer
            drawerAnimator.SetTrigger("CloseDrawer");
            audioSource.enabled = true;
            audioSource.clip = drawerCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the drawer
            drawerAnimator.SetTrigger("OpenDrawer");
            audioSource.enabled = true;
            audioSource.clip = drawerOpenSound;
            audioSource.Play();
            isOpen = true;
        }

        isOnCooldown = true;
        cooldownTimer = CooldownDuration;
    }
}
