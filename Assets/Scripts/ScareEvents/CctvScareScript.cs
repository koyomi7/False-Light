using System.Collections;
using UnityEngine;

public class CctvScareScript : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchOnSound;
    [SerializeField] private AudioClip switchOffSound;
    [SerializeField] private AudioClip scareSound;
    [SerializeField] private GameObject cctvScreen;
    [SerializeField] private GameObject ghost;
    [SerializeField] private Transform ghostTargetPos;
    [SerializeField] private float stareTimeThreshold = 3f;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private GameObject camObject;

    [SerializeField] private Camera playerCamera;
    private bool isTurnedOn = false;
    private float stareTimer = 0f;
    private bool scareTriggered = false;
    private bool scareCompleted = false;

    public void Start()
    {
        cctvScreen.SetActive(false);
        isTurnedOn = false;
        ghost.SetActive(true);
    }

    void Update()
    {
        if (isTurnedOn && !scareTriggered)
        {
            if (IsPlayerLookingAtScreen())
            {
                stareTimer += Time.deltaTime;
                if (stareTimer >= stareTimeThreshold)
                {
                    scareTriggered = true;
                    StartCoroutine(InitialScreenOff());
                }
            }
            else
            {
                stareTimer = 0f;
            }
        }
    }

    public void Interact()
    {
        if (!isTurnedOn)
        {
            cctvScreen.SetActive(true);
            isTurnedOn = true;
            audioSource.clip = switchOnSound;
            audioSource.Play();

            if (scareTriggered && !scareCompleted)
            {
                StartCoroutine(ScareSequence());
            }
            else if (scareCompleted)
            {
                ghost.SetActive(false);
            }
        }
        else
        {
            cctvScreen.SetActive(false);
            isTurnedOn = false;
            audioSource.clip = switchOffSound;
            audioSource.Play();
        }
    }

    bool IsPlayerLookingAtScreen()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
        {
            return hit.transform == cctvScreen.transform;
        }
        return false;
    }

    System.Collections.IEnumerator InitialScreenOff()
    {
        cctvScreen.SetActive(false);
        isTurnedOn = false;
        yield return null;
    }

    System.Collections.IEnumerator ScareSequence()
    {
        ghost.transform.position = ghostTargetPos.position;
        ghost.transform.LookAt(camObject.transform.position);

        audioSource.clip = scareSound;
        audioSource.Play();

        yield return new WaitForSeconds(2f);

        cctvScreen.SetActive(false);
        audioSource.Stop();
        isTurnedOn = false;

        ghost.SetActive(false);
        scareCompleted = true;
    }
}