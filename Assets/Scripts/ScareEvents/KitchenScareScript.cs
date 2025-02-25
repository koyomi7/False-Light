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
    [SerializeField] private GameObject scareTrigger;
    [SerializeField] private GameObject kitchenProps;
    [SerializeField] private Animator kitchenPropsAnimator; 

    void Start()
    {
        AirWall.SetActive(false);
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
        scareTrigger.SetActive(false);
        kitchenProps.SetActive(true);
    }

    public void StartSequence()
    {
        AirWall.SetActive(true);
        StartCoroutine(gracePeriod());
        Ghost.SetActive(true);
        ghostAnimator.Play("Dive");
        ghostAudioSource.clip = crawlingSound;
        ghostAudioSource.Play();
        ghostTrigger.SetActive(false);
    }

    public IEnumerator gracePeriod()
    {
        yield return new WaitForSeconds(0.5f);
    }
    
    public void OnGhostHit()
    {
        kitchenPropsAnimator.Play("Scatter");
    }   
    
    public void OnAnimationComplete()
    {
        ghostAudioSource.Stop(); 
        Destroy(AirWall);
        Destroy(Ghost);
        scareTrigger.SetActive(true);
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
