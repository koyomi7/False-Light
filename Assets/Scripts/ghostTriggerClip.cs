using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostTriggerClip : MonoBehaviour
{
    public enum Triggers
    {
        DownstairsOfficeScare,
        DownstairsBathroomScare
    }
    
    [SerializeField] Triggers trigger;
    [SerializeField] GhostEventManager.Occurrences Occurrence;
    [SerializeField] string eventName;
    [SerializeField] bool oneTimeUse = true;

    bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered && oneTimeUse) return;
        if (!other.CompareTag("Player")) return;
        
        Debug.Log($"Player entered trigger {eventName}");
        ExecuteTrigger(Occurrence);
    }
    
    private void ExecuteTrigger(GhostEventManager.Occurrences occurrence)
    {
        switch (trigger)
        {
            case Triggers.DownstairsOfficeScare:
                if (!GameManager.Instance.CanTriggerEvent(1, occurrence != GhostEventManager.Occurrences.End)) break;
                GameManager.Instance.StartEvent(1);
                GhostEventManager.Instance.DownstairsOfficeScare(occurrence);
                hasBeenTriggered = true;
                break;
            case Triggers.DownstairsBathroomScare:
                if (!GameManager.Instance.CanTriggerEvent(2)) break;
                GameManager.Instance.StartEvent(2);
                GhostEventManager.Instance.DownstairsBathroomScare(occurrence);
                hasBeenTriggered = true;
                GameManager.Instance.EndEvent(2);
                break;
        }
    }
}
