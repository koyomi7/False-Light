using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostTriggerClip : MonoBehaviour
{
    public enum Triggers
    {
        DownstairsOfficeScareStart,
        DownstairsOfficeScareEnd,
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
    }
    
    private void ExecuteTrigger()
    {
        switch (trigger)
        {
            case Triggers.DownstairsOfficeScareStart:
                Debug.Log("Executing downstairs office scare start...");
                if (!GameManager.Instance.CanTriggerEvent(1))
                {
                    Debug.Log("Cannot execute downstairs office scare start");
                    break;
                }
                GameManager.Instance.StartEvent(1);
                GhostEventManager.Instance.DownstairsOfficeScareStart();
                hasBeenTriggered = true;
                break;
            case Triggers.DownstairsOfficeScareEnd:
                Debug.Log("Executing downstairs office scare end...");
                if (!GameManager.Instance.CanTriggerEvent(1, start:false))
                {
                    Debug.Log("Cannot execute downstairs office scare end");
                    break;
                }
                GameManager.Instance.EndEvent(1);
                GhostEventManager.Instance.DownstairsOfficeScareEnd();
                hasBeenTriggered = true;
                break;
        }
    }
}
