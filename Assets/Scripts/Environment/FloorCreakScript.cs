using UnityEngine;
using System.Collections;

public class FloorCreakScript : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] creakingSounds;
    [SerializeField] private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private float minTimeBetweenCreaks = 20f;
    [SerializeField] private float maxTimeBetweenCreaks = 60f;
    [SerializeField] private float playerDetectionRadius = 8f;
    [SerializeField] private float minVolume = 0.2f;
    [SerializeField] private float maxVolume = 0.6f;
    [SerializeField] private bool enableRandomCreaks = true;

    [Header("Position Variation")]
    [SerializeField] private float randomPositionRadius = 2f; // Random position within this radius
    
    private Transform player;
    private Vector3 originalPosition;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure audio source for 3D sound
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 12f;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;

        if (enableRandomCreaks)
            StartCoroutine(RandomCreakCoroutine());
    }

    IEnumerator RandomCreakCoroutine()
    {
        while (enableRandomCreaks)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenCreaks, maxTimeBetweenCreaks));

            if (IsPlayerNearby())
            {
                PlayRandomCreak();
            }
        }
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= playerDetectionRadius;
    }

    private void PlayRandomCreak()
    {
        if (creakingSounds.Length == 0) return;

        // Get random position within radius
        Vector2 randomCircle = Random.insideUnitCircle * randomPositionRadius;
        transform.position = originalPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        AudioClip creakSound = creakingSounds[Random.Range(0, creakingSounds.Length)];
        audioSource.volume = Random.Range(minVolume, maxVolume);
        audioSource.pitch = Random.Range(0.95f, 1.05f); // Slight pitch variation
        audioSource.PlayOneShot(creakSound);

        // Return to original position after playing
        transform.position = originalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, randomPositionRadius);
    }
}