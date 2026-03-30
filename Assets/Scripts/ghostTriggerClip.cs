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
        DownstairsLivingRoomScare,
        DownstairsHallwayScare,
        DownstairsKitchenScare
    }
    
    [SerializeField] Triggers trigger; // The scare scenario that triggers
    [SerializeField, Range(1, 10)] int occurrence; // The occurrence in a sequence of triggers (i.e., 1 = player enters trigger, 2 = player looks at ghost)
    [SerializeField] string eventName; // A description of the scare trigger for developers
    [SerializeField] bool oneTimeUse = true;
    [SerializeField] public bool visualTrigger = false;

    bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered && oneTimeUse) return;
        if (!other.CompareTag("Player")) return;
        if (visualTrigger) return; // Handled in PlayerInteraction.CheckForGhostTrigger(), not during player collision
        
        // Debug.Log($"Player entered trigger {eventName}");
        ExecuteTrigger(occurrence);
    }

    public void VisualTrigger()
    {
        if (hasBeenTriggered && oneTimeUse) return;

        // Debug.Log($"Player looked at trigger {eventName}");
        ExecuteTrigger(occurrence);
    }
    
    void ExecuteTrigger(int occurrence)
    {
        int id = (int)trigger + 1;

        if (!GameManager.Instance.CanTriggerEvent(id, occurrence, occurrence == 1)) return;

        var methodName = trigger.ToString();
        var method = typeof(GhostEventManager).GetMethod(methodName);

        if (method != null)
        {
            var coroutine = (IEnumerator)method.Invoke(GhostEventManager.Instance, new object[] { occurrence });
            StartCoroutine(coroutine);
            hasBeenTriggered = true;
        }
    }
}
