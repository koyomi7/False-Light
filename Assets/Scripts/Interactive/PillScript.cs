using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour {
    public void Interact() {
        Debug.Log("Player ate pills.");
        Destroy(gameObject);
    }
}
