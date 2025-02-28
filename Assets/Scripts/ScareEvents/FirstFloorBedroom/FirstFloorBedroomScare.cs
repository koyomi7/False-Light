using System.Collections;
using UnityEngine;

public class FirstFloorBedroomScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost1; 
    [SerializeField] private GameObject Ghost2;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioSource ghost2AudioSource;
    [SerializeField] private AudioClip FleshSoundEffect;
    [SerializeField] private AudioClip RunAndHitSound;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private Animator ghost1Animator;
    [SerializeField] private Animator ghost2Animator;
    [SerializeField] private GameObject scareTrigger;
    [SerializeField] private GameObject blood;

    void Start()
    {
        ghostTrigger.SetActive(true);   
        Ghost1.SetActive(false);
        Ghost2.SetActive(false);
        scareTrigger.SetActive(false);
        blood.SetActive(false);
    }

    public void StartSequence()
    {
        Ghost1.SetActive(true);
        ghost1Animator.Play("SlowGettingUp");
        ghostAudioSource.PlayOneShot(FleshSoundEffect);
        ghostTrigger.SetActive(false);
    }

    public void OnGettingUpComplete() 
    {
        Debug.Log("Ghost1 finished getting up, spawning Ghost2...");

        Ghost1.SetActive(false); 
        SpawnGhost2();
    }

    

    public void OnRunningAndHittingComplete() 
    {
        Debug.Log("Ghost2 finished running and hitting.");
        Ghost2.SetActive(false);
        Destroy(ghostTrigger);
        scareTrigger.SetActive(true);
        blood.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject == ghostTrigger)
            {
                Debug.Log("GhostTrigger activated.");
                StartSequence();
            }
        }
    }

    private void SpawnGhost2()
    {
        Ghost2.SetActive(true);

        ghost2Animator.Play("RunAndHit");
        ghost2AudioSource.PlayOneShot(RunAndHitSound);
    }
}
