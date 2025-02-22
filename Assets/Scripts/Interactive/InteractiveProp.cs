using UnityEngine;

public class InteractiveProp : MonoBehaviour, IInteractable {
    public void Interact() {
        FindObjectOfType<PlayerScript>().Pickup(gameObject);
    }
}
