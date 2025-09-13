using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    [HideInInspector] public int pills = 0;
    int eventIdPlaying = 0;
    bool isPlaying = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioManager = GetComponentInChildren<AudioManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanTriggerEvent(int eventId, bool start = true)
    {
        return eventIdPlaying == (start ? 0 : eventId) && !isPlaying;
    }

    public void StartEvent(int eventId)
    {
        eventIdPlaying = eventId;
        isPlaying = true;
        Debug.Log($"Event started (eventId = {eventId})");
    }

    public void EndEvent(int eventId)
    {
        eventIdPlaying = 0;
        isPlaying = false;
        Debug.Log($"Event ended (eventId = {eventId})");
    }

    public void EndEventReady()
    {
        isPlaying = false;
    }

    public void UpdatePillsCount(int consumed = 1)
    {
        pills += consumed;
    }
}