using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    
    public int pills = 0;
    
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

    public void UpdatePillsCount(int consumed = 1)
    {
        pills += consumed;
    }
}