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
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            // if (hit.collider.isTrigger) continue;

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.gameObject.SetActive(true);

                for (int j = i; j < hits.Length; j++)
                {
                    if (!((interactableLayer.value & (1 << hit.collider.gameObject.layer)) > 0))
                    {
                        break;
                    }

                    if (hits[j].collider.CompareTag("Pill"))
                    {
                        currentInteractable = hits[j].collider.GetComponent<IInteractable>();
                        break;
                    }
                }

                return;
            }
            else
            {
                if (hit.collider.CompareTag("PlayerClip")) continue;
                currentInteractable = null;
                interactionText.gameObject.SetActive(false);
                return;
            }
        }

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
