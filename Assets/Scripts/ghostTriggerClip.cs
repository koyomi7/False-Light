using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostTriggerClip : MonoBehaviour
{
    public enum Triggers
    {
        None,
        DownstairsOfficeScare1,
        DownstairsOfficeScare2,
    }
    
    [SerializeField] Triggers trigger;
    [SerializeField] string eventName;
    [SerializeField] bool oneTimeUse = true;

    bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered && oneTimeUse) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Player entered trigger {eventName}");
        ExecuteTrigger();
        
        if (oneTimeUse) hasBeenTriggered = true;
    }
    
    private void ExecuteTrigger()
    {
        switch (trigger)
        {
            case Triggers.None:
                Debug.Log("None");
                break;

            case Triggers.DownstairsOfficeScare1:
                Debug.Log("Activating downstairs office scare 1");
                break;
            case Triggers.DownstairsOfficeScare2:
                Debug.Log("Activating downstairs office scare 2");
                break;
        }
    }
}
