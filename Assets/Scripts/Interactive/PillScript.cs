using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour, IInteractable {
    public void Interact() {
        GameManager.Instance.UpdatePillsCount();
        Debug.Log("Player ate pills (" + GameManager.Instance.pills + ")");
        Destroy(gameObject);
    }
}
