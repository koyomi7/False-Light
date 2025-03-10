using UnityEngine;
using TMPro; // Required for TextMeshProUGUI

public class KeyInventory : MonoBehaviour
{
    [SerializeField] private int keyCount = 0; 
    [SerializeField] private TextMeshProUGUI keyCountText; 
    public static KeyInventory Instance { get; private set; } 

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateKeyCountUI();
    }

    public bool HasKey()
    {
        return keyCount > 0;
    }

    public void AddKey()
    {
        keyCount++;
        Debug.Log($"Key acquired! Total keys: {keyCount}");
        UpdateKeyCountUI(); 
    }

    public void UseKey()
    {
        if (HasKey())
        {
            keyCount--;
            Debug.Log($"Key used! Remaining keys: {keyCount}");
            UpdateKeyCountUI();
        }
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    private void UpdateKeyCountUI()
    {
        if (keyCountText != null)
        {
            keyCountText.text = $"    : {keyCount}";
            keyCountText.gameObject.SetActive(keyCount > 0); // Show only if keys > 0
        }
    }
}