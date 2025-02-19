using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestRoomScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource ghostAudioSource; 
    [SerializeField] private AudioSource footstepAudioSource; 
    [SerializeField] private AudioClip HeavyBreathing;
    [SerializeField] private AudioClip GhostRoar;
    [SerializeField] private AudioClip GhostFootsteps;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private GameObject scareTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private float ghostSpeed = 0.7f;

    private bool isGhostRunning = false;

    void Start()
    {
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
        scareTrigger.SetActive(false);
    }

    public void GhostAppear()
    {
        Ghost.SetActive(true);
        ghostAnimator.Play("Seizure");
        ghostAudioSource.clip = HeavyBreathing;
        ghostAudioSource.Play();
        ghostTrigger.SetActive(false);
        scareTrigger.SetActive(true);
    }

    void Update()
    {
        if (isGhostRunning)
        {
            RunToPlayer();
        }
    }

    public void GhostDisappear()
    {
        Ghost.SetActive(false);
        ghostAudioSource.Stop();
        footstepAudioSource.Stop(); 
        StartCoroutine(ReappearAndRun());
    }

    private IEnumerator ReappearAndRun()
    {
        yield return new WaitForSeconds(1f); 

        // Reappear the ghost
        Ghost.transform.position = new Vector3(Ghost.transform.position.x, 0f, Ghost.transform.position.z);
        Ghost.transform.rotation = Quaternion.Euler(180, 180, 0);
        Ghost.SetActive(true);
        ghostAnimator.Play("ClownRun");

        ghostAudioSource.clip = GhostRoar;
        ghostAudioSource.Play();
        footstepAudioSource.clip = GhostFootsteps;
        footstepAudioSource.loop = true;
        footstepAudioSource.Play();

        isGhostRunning = true;
    }

    private void RunToPlayer()
    {
        Vector3 direction = (player.position - Ghost.transform.position).normalized;
        direction.y = 0; // Prevent ghost from moving up or down
        Ghost.transform.position += direction * ghostSpeed * Time.deltaTime;
        Ghost.transform.LookAt(player);

        if (Vector3.Distance(Ghost.transform.position, player.position) < 0.5f)
        {
            isGhostRunning = false;
            ghostAudioSource.Stop();
            footstepAudioSource.Stop();
            Destroy(Ghost);
            Destroy(scareTrigger);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject == ghostTrigger)
            {
                GhostAppear();
                Debug.Log("Ghost Triggered");
            }
            else if (gameObject == scareTrigger)
            {
                Debug.Log("Scare Triggered");
                GhostDisappear();
            }
        }
    }
}
