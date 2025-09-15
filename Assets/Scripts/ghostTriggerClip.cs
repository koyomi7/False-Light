using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostTriggerClip : MonoBehaviour
{
    public enum Triggers
    {
        DownstairsOfficeScare,
        DownstairsBathroomScare,
        DownstairsBedroomScare,
        DownstairsLivingRoomScare
    }
    
    [SerializeField] Triggers trigger;
    [SerializeField, Range(1, 10)] int occurrence;
    [SerializeField] string eventName;
    [SerializeField] bool oneTimeUse = true;
    [SerializeField] public bool visualTrigger = false;

    bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered && oneTimeUse) return;
        if (!other.CompareTag("Player")) return;
        if (visualTrigger) return;
        
        Debug.Log($"Player entered trigger {eventName}");
        ExecuteTrigger(occurrence);
    }

    public void VisualTrigger()
    {
        if (hasBeenTriggered && oneTimeUse) return;

        Debug.Log($"Player looked at trigger {eventName}");
        ExecuteTrigger(occurrence);
    }
    
    void ExecuteTrigger(int occurrence)
    {
        switch (trigger)
        {
            case Triggers.DownstairsOfficeScare:
                if (!GameManager.Instance.CanTriggerEvent(1, occurrence, occurrence == 1)) break;
                StartCoroutine(GhostEventManager.Instance.DownstairsOfficeScare(occurrence));
                hasBeenTriggered = true;
                break;
            case Triggers.DownstairsBathroomScare:
                if (!GameManager.Instance.CanTriggerEvent(2, occurrence, occurrence == 1)) break;
                StartCoroutine(GhostEventManager.Instance.DownstairsBathroomScare(occurrence));
                hasBeenTriggered = true;
                break;
            case Triggers.DownstairsBedroomScare:
                if (!GameManager.Instance.CanTriggerEvent(3, occurrence, occurrence == 1)) break;
                StartCoroutine(GhostEventManager.Instance.DownstairsBedroomScare(occurrence));
                hasBeenTriggered = true;
                break;
            case Triggers.DownstairsLivingRoomScare:
                if (!GameManager.Instance.CanTriggerEvent(4, occurrence, occurrence == 1)) break;
                StartCoroutine(GhostEventManager.Instance.DownstairsLivingRoomScare(occurrence));
                hasBeenTriggered = true;
                break;
        }
    }
}
