using System.Collections;
using UnityEngine;

public class DoubleGhostScare : MonoBehaviour
{
    [SerializeField] private GameObject ghost1;
    [SerializeField] private GameObject ghost2;
    [SerializeField] private GameObject scareTrigger1;
    [SerializeField] private Animator ghost1Animator;
    [SerializeField] private Animator ghost2Animator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask ghostLayer;


    void Start()
    {
        ghost1.SetActive(false);
        ghost2.SetActive(false);
        scareTrigger1.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerGhost1Fall();
        }
    }

    void Update()
    {
        CheckIfPlayerLookingAtGhost2();
    }

    private void TriggerGhost1Fall()
    {
        scareTrigger1.SetActive(false);
        ghost1.SetActive(true);
        ghost1Animator.Play("FallingOnImpact"); 
    }

    private void DropGhost1()
    {
        ghost1.SetActive(false); 
        ActivateGhost2();
    }

    private void ActivateGhost2()
    {
        ghost2.SetActive(true);
    }

    private void CheckIfPlayerLookingAtGhost2()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.yellow);

        if (Physics.Raycast(ray, out hit, 20f, ghostLayer))
        {
            if (hit.collider.gameObject == ghost2)
            {
                Debug.Log("Player looked at Ghost 2!");
                ghost2Animator.Play("Vanish");
            }
        }
    }   

    public void OnGhost2Vanish()
    {
        Destroy(ghost2);
        Destroy(ghost1);
        Destroy(scareTrigger1);
    }
}
