using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    [SerializeField] private AudioSource glassAudioSource;
    [SerializeField] private AudioClip glassBreak;
    [SerializeField] private GameObject glassTrigger;

    void Start()
    {
        glassTrigger.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            glassAudioSource.clip = glassBreak;
            glassAudioSource.Play();
            glassTrigger.SetActive(false);
        }
    }
}
