using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingGhostTrigger : MonoBehaviour
{
    [SerializeField] public GameObject StalkingGhost;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StalkingGhost.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StalkingGhost.SetActive(false);
        }
    }
}
