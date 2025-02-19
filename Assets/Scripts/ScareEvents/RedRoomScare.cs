using UnityEngine;

public class GhostRetreatScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip footSteps;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask ghostLayer; 
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private GameObject endingTrigger;

    private bool isRetreating = false;

    void Start()
    {
        Ghost.SetActive(false);
        endingTrigger.SetActive(false);
        ghostTrigger.SetActive(true);
    }

    void Update()
    {
        CheckIfPlayerLooking();
    }

    private void CheckIfPlayerLooking()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of screen
        RaycastHit hit;

        // Debug ray
        //Debug.DrawRay(ray.origin, ray.direction * 20f, Color.yellow);

        if (Physics.Raycast(ray, out hit, 20f, ghostLayer))
        {
            if (hit.collider.gameObject == Ghost)
            {
                if (!isRetreating)
                {
                    StartRetreat();
                }
            }
        }
    }

    private void StartRetreat()
    {
        isRetreating = true;
        ghostAnimator.Play("WalkingBack");
        ghostAudioSource.clip = footSteps;
        ghostAudioSource.Play();
        endingTrigger.SetActive(true);  
    }

    private void StartSequence()
    {
        Ghost.SetActive(true);
        ghostTrigger.SetActive(false);
        Debug.Log("Ghost appeared");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject == ghostTrigger)
            {
                StartSequence();
            }
            else if (gameObject == endingTrigger)
            {
                Destroy(Ghost);
                Destroy(ghostTrigger);
                Destroy(endingTrigger);
            }
        }
    }
}