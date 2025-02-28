using System.Collections;
using UnityEngine;

public class AfterShock : MonoBehaviour
{
    [SerializeField] private GameObject Ghost; 
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip BloodLeakSound;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask ghostLayer; 
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject ghostTrigger; 

    void Start()
    {
        Ghost.SetActive(false);
        ghostTrigger.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnGhost();
        }
    }

    

    void Update()
    {
        CheckIfPlayerLookingAtGhost();
    }

    private void CheckIfPlayerLookingAtGhost()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red);

        if (Physics.Raycast(ray, out hit, 20f, ghostLayer))
        {
            if (hit.collider.gameObject == Ghost)
            {
                Debug.Log("Player is looking at Ghost3, triggering CrawlBack...");
                ghostAnimator.Play("CrawlBack");
            }
        }
    }

    public void OnCrawlBackComplete() 
    {
        Debug.Log("Final scare event complete. Cleaning up...");
        Destroy(Ghost);
        Destroy(ghostTrigger);
        Destroy(gameObject);
    }

    private void SpawnGhost()
    {
        Debug.Log("Ghost3 spawned for final scare!");
        Ghost.SetActive(true);
        ghostAudioSource.PlayOneShot(BloodLeakSound);
    }
}
