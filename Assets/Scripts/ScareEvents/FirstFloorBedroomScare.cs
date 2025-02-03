using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFloorBedroomScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip FleshSoundEffect;
    [SerializeField] private AudioClip ChokingSoundEffect;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject AirWall;
    [SerializeField] private GameObject scareTrigger;

    void Start()
    {
        AirWall.SetActive(false);
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
        scareTrigger.SetActive(false);
    }

    public void StartSequence()
    {
        AirWall.SetActive(true);
        Ghost.SetActive(true);
        ghostAnimator.Play("SlowGettingUp");
        ghostAudioSource.clip = FleshSoundEffect;
        ghostAudioSource.Play();
        ghostTrigger.SetActive(false);
        scareTrigger.SetActive(true);
    }

    public void OnAnimationComplete()
    {
        ghostAudioSource.Stop(); 
        Destroy(AirWall);
        Ghost.SetActive(false);
    }

    public void OnChokingAnimationComplete()
    {
        ghostAudioSource.Stop();
        Destroy(Ghost); 
    }

    public void StartAfterShock()
{
    Vector3 playerPosition = player.transform.position;
    Vector3 ghostTargetPosition = playerPosition + player.transform.forward * 1.5f;
    ghostTargetPosition.y = 0.5f;

    Ghost.transform.position = ghostTargetPosition;
    Ghost.transform.LookAt(playerPosition); 

    Ghost.SetActive(true);
    ghostAnimator.Play("chocking");
    ghostAudioSource.clip = ChokingSoundEffect;
    ghostAudioSource.volume = 1f;
    ghostAudioSource.Play();
    scareTrigger.SetActive(false);
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject == ghostTrigger){
                StartSequence();
            }
            else if (gameObject == scareTrigger){
                StartAfterShock();
            }
            
        }
    }
}
