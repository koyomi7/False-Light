using System.Collections;
using UnityEngine;

public class LivingRoomScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private GameObject ghostLight;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioSource TriggerAudioSource;
    [SerializeField] private AudioSource EndingAudioSource; 
    [SerializeField] private AudioClip gettingHitSound;
    [SerializeField] private AudioClip[] creepySounds;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject AirWall;
    [SerializeField] private GameObject endingTrigger;


    void Start()
    {
        ghostLight.SetActive(true);
        AirWall.SetActive(true);
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
        endingTrigger.SetActive(true);
    }

    public void StartSequence()
    {
        Ghost.SetActive(true);
        ghostAnimator.Play("GettingHit");
        TriggerAudioSource.clip = creepySounds[1];
        TriggerAudioSource.Play();
        ghostTrigger.SetActive(false);
    }

    public void PlayHitSound()
    {
        ghostAudioSource.clip = gettingHitSound;
        ghostAudioSource.Play();
    }

    public void OnAnimationComplete()
    {
        ghostAudioSource.Stop();
        AirWall.SetActive(false);
    }

    public void EndingTriggered()
    {
        EndingAudioSource.clip = creepySounds[0];
        EndingAudioSource.Play();
        
        Destroy(AirWall);
        Destroy(Ghost);
        Destroy(ghostLight);
        Destroy(ghostTrigger);
        Destroy(endingTrigger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject == ghostTrigger)
            {
                StartSequence();
            }
            else if (gameObject == endingTrigger)
            {
                EndingTriggered();
            }
        }
    }
}