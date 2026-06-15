using UnityEngine;

public class KeysPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip pickupSound; 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject keyModel; 

    public void Interact()
    {
        KeyInventory.Instance.AddKey();
        audioSource.PlayOneShot(pickupSound);

        Debug.Log("Key picked up!");
        Destroy(keyModel);
        //turn off collider
        GetComponent<Collider>().enabled = false;
    }
}