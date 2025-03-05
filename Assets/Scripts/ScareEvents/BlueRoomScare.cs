using System.Collections;
using UnityEngine;

public class BlueRoomScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private GameObject ghostTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private Animator windowAnimator;
    [SerializeField] private AudioSource windowAudioSource;
    [SerializeField] private AudioClip windowTappingSound;
    [SerializeField] private AudioClip windowBreakInSound;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask ghostLayer;
    private float yOffsetAfterSwimUp = 2f; 

    private bool canCheckPlayerLooking = false;

    void Start()
    {
        ghostTrigger.SetActive(true);
        Ghost.SetActive(false);
    }

    void Update()
    {
        if (canCheckPlayerLooking)
        {
            CheckIfPlayerLookingAtGhost();
        }
    }

    public void StartSequence()
    {
        Ghost.SetActive(true);
        ghostTrigger.SetActive(false);
        ghostAnimator.Play("SwimUp");
    }

    public void OnSwimUpComplete() 
    {
        Debug.Log("SwimUp complete, adjusting position and enabling look detection.");

        
        // Enable look detection
        canCheckPlayerLooking = true;
    }

    public void OnGhostInPosition()
    {
        Debug.Log("Ghost in position, starting window tapping...");
        windowAudioSource.PlayOneShot(windowTappingSound);
    }

    public void OnGhostBreakIn()
    {
        windowAnimator.Play("BrokenInto");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GhostTrigger activated.");
            StartSequence();
        }
    }

    private void CheckIfPlayerLookingAtGhost()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.yellow);

        if (Physics.Raycast(ray, out hit, 20f, ghostLayer))
        {
            if (hit.collider.gameObject == Ghost)
            {
                Ghost.transform.parent.position += new Vector3(0, yOffsetAfterSwimUp, 0);
                Debug.Log("Player is looking at Ghost, triggering break in...");
                ghostAnimator.Play("BreakInWindow");
                windowAudioSource.PlayOneShot(windowBreakInSound);
                canCheckPlayerLooking = false; // Stop checking after event is triggered
            }
        }
    }

    public void OnWindowBreakInComplete()
    {
        Debug.Log("Window break in complete. Cleaning up...");
        Destroy(Ghost);
        Destroy(ghostTrigger);
        Destroy(gameObject);
    }
}
