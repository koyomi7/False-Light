using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    
    [HideInInspector] public int pills = 0;

    // Event trigger cooldown system
    bool isEventPlaying = false;
    float eventCooldownTimer = 0f;
    [SerializeField] public float eventCooldownDuration;
    
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

    void Update()
    {
        HandleEventCooldown();
    }

    void HandleEventCooldown()
    {
        if (eventCooldownTimer > 0f)
        {
            eventCooldownTimer -= Time.deltaTime;
            if (eventCooldownTimer <= 0f) isEventPlaying = false;
        }
    }

    public bool CanTriggerEvent()
    {
        return !isEventPlaying;
    }

    public void StartEvent()
    {
        isEventPlaying = true;
        eventCooldownTimer = eventCooldownDuration;
        Debug.Log("Event started");
    }

    public void UpdatePillsCount(int consumed = 1)
    {
        pills += consumed;
    }
}