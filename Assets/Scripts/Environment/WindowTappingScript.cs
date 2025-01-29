using UnityEngine;
using System.Collections;

public class WindowTappingScript : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] tappingSounds;
    [SerializeField] private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private float minTimeBetweenTaps = 60f;
    [SerializeField] private float maxTimeBetweenTaps = 150f;
    [SerializeField] private float playerDetectionRadius = 7f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private bool enableRandomTaps = true;

    private Transform player;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure audio source for 3D sound
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enableRandomTaps)
            StartCoroutine(RandomTappingCoroutine());
    }

    IEnumerator RandomTappingCoroutine()
    {
        while (enableRandomTaps)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenTaps, maxTimeBetweenTaps));

            if (IsPlayerNearby())
            {
                PlayRandomTap();
            }
        }
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= playerDetectionRadius;
    }

    private void PlayRandomTap()
    {
        if (tappingSounds.Length == 0) return;

        AudioClip tapSound = tappingSounds[Random.Range(0, tappingSounds.Length)];
        audioSource.volume = Random.Range(minVolume, maxVolume);
        audioSource.pitch = Random.Range(0.95f, 1.05f); // Slight pitch variation
        audioSource.PlayOneShot(tapSound);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}