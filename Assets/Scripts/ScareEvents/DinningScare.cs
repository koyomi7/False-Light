using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinningScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioSource glitchBackgroud;
    [SerializeField] private AudioClip glitchedBreathing;
    [SerializeField] private AudioClip glitchedBackground;
    [SerializeField] private GameObject endingTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
        Ghost.SetActive(false);
        scareTrigger.SetActive(true);
        endingTrigger.SetActive(false);
    }

    public void StartSequence()
    {
        Ghost.SetActive(true);
        ghostAnimator.Play("GlichtedSitting");
        //play both clips
        ghostAudioSource.clip = glitchedBreathing;
        ghostAudioSource.Play();
        glitchBackgroud.clip = glitchedBackground;
        glitchBackgroud.Play();
        scareTrigger.SetActive(false);
        endingTrigger.SetActive(true);
    }

    public void OnPlayerExitRoom()
    {
        ghostAudioSource.Stop(); 
        Destroy(Ghost);
        Destroy(endingTrigger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject == scareTrigger){
                StartSequence();
            }
            else if (gameObject == endingTrigger)
            {
                OnPlayerExitRoom();
            }
        }
    }
}
