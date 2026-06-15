using UnityEngine;
using UnityEngine.Audio; // For Audio Mixer
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Fade Settings")]
    [SerializeField] private float fadeInDuration = 1.5f; 
    [SerializeField] private AudioMixer mainMixer; 
    [SerializeField] private float startVolume = -80f; 
    [SerializeField] private float targetVolume = 0f; 

    private void Start()
    {
        // Start with muted audio
        mainMixer.SetFloat("MasterVolume", startVolume);
        
        // Begin fade in
        StartCoroutine(FadeInAudio());
    }

    private IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeInDuration);
            mainMixer.SetFloat("MasterVolume", newVolume);
            yield return null;
        }

        // Ensure we end up exactly at target volume
        mainMixer.SetFloat("MasterVolume", targetVolume);
    }
}