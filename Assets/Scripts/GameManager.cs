using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    [HideInInspector] public int pills = 0;
    int eventIdPlaying = 0;
    int occurrence = 0;
    
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

    public bool CanTriggerEvent(int eventId, int occurrence, bool start = true)
    {
        return eventIdPlaying == (start ? 0 : eventId) && this.occurrence == occurrence - 1;
    }

    public void StartEvent(int eventId)
    {
        eventIdPlaying = eventId;
        Debug.Log($"Event started (eventId = {eventId})");
    }

    public void EndEvent(int eventId)
    {
        eventIdPlaying = 0;
        occurrence = 0;
        Debug.Log($"Event ended (eventId = {eventId})");
    }

    public void NextEventReady(int next = 1)
    {
        occurrence += next;
    }

    public void UpdatePillsCount(int consumed = 1)
    {
        pills += consumed;
    }
}