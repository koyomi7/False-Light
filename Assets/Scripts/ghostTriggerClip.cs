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
        DownstairsKitchenScare,
        DownstairsSecretScare
    }
    
    [SerializeField] Triggers trigger; // The scare scenario that triggers
    [SerializeField, Range(1, 10)] int occurrence; // The occurrence in a sequence of triggers (i.e., 1 = player enters trigger, 2 = player looks at ghost)
    [SerializeField] string eventName; // A description of the scare trigger for developers
    [SerializeField] bool oneTimeUse = true;
    [SerializeField] public bool visualTrigger = false;
    [SerializeField] bool progressBar = false;
    [SerializeField, Range(0.1f, 5f)] float progressBarFillSpeed = 1f;
    [SerializeField, Range(0.1f, 5f)] float progressBarDrainSpeed = 2f;
    [SerializeField] string progressBarFlagName;
    [SerializeField] bool debug = false;

    bool hasBeenTriggered = false;
    bool incrementProgressBar = false;
    float progressBarValue = 0f;
    
    void Update()
    {
        UpdateProgressBar();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!CanTriggerEvent())             return; // Trigger is inactive
        if (hasBeenTriggered && oneTimeUse) return; // Trigger has already been triggered
        if (!other.CompareTag("Player"))    return; // Trigger is not being triggered by the player
        
        if (debug) Debug.Log($"Player entered trigger {eventName}");
        if (progressBar) incrementProgressBar = true;
        else ExecuteTrigger();
    }

    void OnTriggerExit(Collider other)
    {
        if (!CanTriggerEvent())             return; // Trigger is inactive
        if (hasBeenTriggered && oneTimeUse) return; // Trigger has already been triggered
        if (!other.CompareTag("Player"))    return; // Trigger is not being triggered by the player

        if (debug) Debug.Log($"Player exited trigger {eventName}");
        if (progressBar) incrementProgressBar = false;
    }

    public void VisualTrigger(bool isLookingAtTrigger)
    {
        if (!CanTriggerEvent())             return; // Trigger is inactive
        if (hasBeenTriggered && oneTimeUse) return; // Trigger has already been triggered

        if (isLookingAtTrigger)
        {
            if (debug) Debug.Log($"Player is looking at trigger {eventName}");
            if (progressBar) incrementProgressBar = true;
            else ExecuteTrigger();
        }
        else
        {
            if (debug) Debug.Log($"Player is NOT looking at trigger {eventName}");
            if (progressBar) incrementProgressBar = false;
        }
    }

    public bool CanTriggerEvent()
    {
        int id = (int)trigger + 1;

        return GameManager.Instance.CanTriggerEvent(id, occurrence, occurrence == 1);
    }
    
    void ExecuteTrigger()
    {
        var methodName = trigger.ToString();
        var method = typeof(GhostEventManager).GetMethod(methodName);

        if (method != null)
        {
            var coroutine = (IEnumerator)method.Invoke(GhostEventManager.Instance, new object[] { occurrence });
            StartCoroutine(coroutine);
            hasBeenTriggered = true;
        }
    }

    void UpdateProgressBar()
    {
        if (!progressBar)                   return; // Trigger does not have a progress bar
        if (hasBeenTriggered && oneTimeUse) return; // Trigger has already been triggered

        if (incrementProgressBar)
        {
            progressBarValue = Mathf.MoveTowards(progressBarValue, 1f, progressBarFillSpeed * Time.deltaTime);
            if (progressBarValue >= 1f)
            {
                progressBarValue = 1f;
                ExecuteTrigger();
            }
        }
        else progressBarValue = Mathf.MoveTowards(progressBarValue, 0f, progressBarDrainSpeed * Time.deltaTime);

        var manager = GhostEventManager.Instance;
        var field = manager.GetType().GetField(progressBarFlagName);

        if (field != null) field.SetValue(manager, progressBarValue);
        
        if (debug) Debug.Log($"Progress bar value: {progressBarValue}");
    }
}
