using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YogaBall : MonoBehaviour, IInteractable
{
    [SerializeField] float pushStrength = 1.0f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Interact() {
        Debug.Log("Player pushed the yoga ball!");
        Vector3 pushDir = Camera.main.transform.forward;
        pushDir.Normalize();
        rb.AddForce(pushDir * pushStrength, ForceMode.Impulse);
    }
}
