using UnityEngine;

public class ScareBathroomTriggerScript : MonoBehaviour
{
    [SerializeField] private ScareBathroomDoorScript bathroomDoor ; // Reference to the door script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            if (bathroomDoor != null)
            {
                bathroomDoor.PlayScareEvent();
            }
            else
            {
                Debug.LogError("Door Script reference is missing!");
            }
        }
    }
}