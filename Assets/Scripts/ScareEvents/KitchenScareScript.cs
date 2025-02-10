using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenScareScript : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip crawlingSound;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject AirWall;

    void Start()
    {
        AirWall.SetActive(false);
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
    }

    public void StartSequence()
    {
        AirWall.SetActive(true);
        Ghost.SetActive(true);
        ghostAnimator.Play("SlowCrawl");
        ghostAudioSource.clip = crawlingSound;
        ghostAudioSource.Play();
        ghostTrigger.SetActive(false);
    }

    public void OnAnimationComplete()
    {
        ghostAudioSource.Stop(); 
        Destroy(AirWall);
        Ghost.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject == ghostTrigger){
                StartSequence();
            }
        }
    }
}
