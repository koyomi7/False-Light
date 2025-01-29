using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkDrawerScript : MonoBehaviour
{
    [SerializeField] private Animator sinkDrawerAnimator;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sinkDrawerOpenSound;
    [SerializeField] private AudioClip sinkDrawerCloseSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float CooldownDuration = 1f;

    void Start()
    {
        sinkDrawerAnimator = GetComponent<Animator>();
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
            return; // Exit the method if the sink drawer is on cooldown
        }

        if (isOpen)
        {
            // Close the sink drawer
            sinkDrawerAnimator.SetTrigger("CloseSinkDrawer");
            audioSource.enabled = true;
            audioSource.clip = sinkDrawerCloseSound;
            audioSource.Play();

            isOpen = false;
        }
        else
        {
            // Open the sink drawer
            sinkDrawerAnimator.SetTrigger("OpenSinkDrawer");
            audioSource.enabled = true;
            audioSource.clip = sinkDrawerOpenSound;
            audioSource.Play();
            isOpen = true;
        }
    }
}
