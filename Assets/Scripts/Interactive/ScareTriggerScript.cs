using UnityEngine;

public class ScareTriggerScript : MonoBehaviour
{
    [SerializeField] private ScareDoorScript scareDoor; // Reference to the door script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            if (scareDoor != null)
            {
                scareDoor.PlayScareEvent();
            }
            else
            {
                Debug.LogError("Door Script reference is missing!");
            }
        }
    }
}