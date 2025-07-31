using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactionDistance;
    [SerializeField] float throwForce;

    [Header("Other")]
    [SerializeField] Transform holdPoint;
    [SerializeField] Camera playerCam;
    [SerializeField] TMPro.TextMeshProUGUI interactionText;
    [SerializeField] LayerMask interactableLayer;

    GameObject heldObject;
    IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractable();
        HandleInteraction();
        // Debug.Log(heldObject);
    }

    public void Pickup(GameObject obj)
    {
        if (heldObject == null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // Disable physics

            obj.transform.SetParent(holdPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            heldObject = obj;
            currentInteractable = null;
            interactionText.gameObject.SetActive(false);
            Debug.Log("PICKED UP OBJECT");
        }
    }

    public void Drop()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics
                rb.AddForce(holdPoint.forward * throwForce, ForceMode.Impulse); // Throw
            }

            heldObject.transform.SetParent(null); // Unparent
            heldObject = null;
            Debug.Log("DROPPED OBJECT");
        }
    }

    void CheckForInteractable()
    {
        if (heldObject != null) return;
        
        // Get camera's forward vector and its pitch angle
        Vector3 cameraForward = playerCam.transform.forward;
        float pitchAngle = Vector3.Angle(Vector3.forward, new Vector3(cameraForward.x, 0, cameraForward.z).normalized);
        
        // Calculate dynamic interaction distance based on camera pitch
        // When looking more up/down (higher pitch), use longer distance
        float dynamicDistance = interactionDistance * (1 + Mathf.Abs(Mathf.Sin(pitchAngle * Mathf.Deg2Rad)) * 0.5f);
        
        Ray ray = new Ray(playerCam.transform.position, cameraForward);
        RaycastHit[] hits = Physics.RaycastAll(ray, dynamicDistance);
        Debug.DrawRay(playerCam.transform.position, cameraForward * dynamicDistance, Color.green);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("PlayerClip")) continue;
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.gameObject.SetActive(true);
                return;
            }
            else
            {
                // Hit something that's not interactable
                currentInteractable = null;
                interactionText.gameObject.SetActive(false);
                return;
            }
        }

        // Didn't hit anything interactable
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }
    
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject != null)
            {
                Drop();
            }
            else if (currentInteractable != null)
            {
                // Check if the interactable is a door requiring a key
                GenericAccessMechanismScript door = currentInteractable as GenericAccessMechanismScript;
                if (door != null && door.requiresKey && !door.isUnlocked)
                {
                    // if (!KeyInventory.Instance.HasKey())
                    // {
                    //     ShowKeysNeededText();
                    //     KeysAudioSource.clip = OpenDoorFailedSound;
                    //     KeysAudioSource.Play();
                    // }
                    // else
                    // {
                        currentInteractable.Interact();
                    //     KeysAudioSource.clip = UnlockDoorSound;
                    //     KeysAudioSource.Play();
                    // }
                }
                else
                {
                    currentInteractable.Interact(); 
                }
                
            }
        }
    }
}
