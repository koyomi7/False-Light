using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour, IInteractable {
    public void Interact() {
        Debug.Log("Player ate pills.");
        Destroy(gameObject);
    }
}
