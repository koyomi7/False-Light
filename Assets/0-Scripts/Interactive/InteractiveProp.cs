using UnityEngine;

public class InteractiveProp : MonoBehaviour, IInteractable {
    public void Interact() {
        FindObjectOfType<PlayerInteraction>().Pickup(gameObject);
    }
}
