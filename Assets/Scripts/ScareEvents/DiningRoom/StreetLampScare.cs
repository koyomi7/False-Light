using UnityEngine;
using System.Collections;

public class LampGhostScare : MonoBehaviour
{
    [SerializeField] private Light streetLamp; 
    [SerializeField] private GameObject ghost; 
    [SerializeField] private Transform ghostTargetPos; 
    [SerializeField] private float flickerSpeed = 0.1f; 
    [SerializeField] private LayerMask raycastMask;
    
    [SerializeField] private Animator ghostAnimator; 
    private bool playerInTrigger = false;
    private bool scareTriggered = false;
    private float flickerTimer;
    [SerializeField] private Camera playerCamera;

    void Start()
    { 
        ghost.SetActive(false);
        this.gameObject.SetActive(false);
        flickerTimer = flickerSpeed;
    }

    void Update()
    {
        FlickerLamp();

        if (playerInTrigger && !scareTriggered && IsPlayerLookingAtLampRaycast())
        {
            scareTriggered = true;
            StartCoroutine(ScareSequence());
        }
    }

    void FlickerLamp()
    {
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0)
        {
            streetLamp.enabled = !streetLamp.enabled;
            flickerTimer = flickerSpeed;
        }
    }

    bool IsPlayerLookingAtLampRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
        {
            if (hit.transform == streetLamp.transform)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator ScareSequence()
    {
        ghost.SetActive(true);
        yield return new WaitForSeconds(1f);
        ghost.SetActive(false);

        yield return new WaitForSeconds(2f);

        ghost.transform.position = ghostTargetPos.position;
        ghost.SetActive(true);
        ghostAnimator.Play("Sad");
        yield return new WaitForSeconds(2f);
        ghost.SetActive(false);

        scareTriggered = false;

        Destroy(ghost);
        enabled = false;    
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    void CleanUp()
    {
        Destroy(this.gameObject);
    }
}