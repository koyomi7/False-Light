using System.Collections;
using UnityEngine;

public class DinningScare : MonoBehaviour
{
    [SerializeField] private GameObject Ghost;
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioSource glitchBackgroud;
    [SerializeField] private AudioClip glitchedBreathing;
    [SerializeField] private AudioClip glitchedBackground;
    [SerializeField] private GameObject endingTrigger;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private GameObject scareTrigger;
    [SerializeField] private GameObject streetLampScareTrigger;

    [Header("Stare Mechanics")]
    [SerializeField] private float maxStareTime = 3f; 
    [SerializeField] private Transform teleportTarget; 
    [SerializeField] private CanvasGroup blackoutPanel; 
    // [SerializeField] private PostProcessVolume postProcessVolume; 
    [SerializeField] private LayerMask ghostLayer; 
    [SerializeField] private float shakeIntensityMax = 0.3f; 
    private float stareTime = 0f;
    // private Vignette vignette;
    private Transform playerTransform;
    private Camera playerCamera; 
    private Vector3 originalCameraLocalPos; // Store original camera position

    void Start()
    {
        Ghost.SetActive(false);
        scareTrigger.SetActive(true);
        endingTrigger.SetActive(false);

        // Initialize post-processing
        // if (postProcessVolume != null)
        // {
        //     postProcessVolume.profile.TryGetSettings(out vignette);
        //     if (vignette != null) vignette.intensity.value = 0f;
        // }

        // Hide blackout panel
        if (blackoutPanel != null)
        {
            blackoutPanel.alpha = 0f;
            blackoutPanel.gameObject.SetActive(false);
        }

        // Find player and camera
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerCamera = playerTransform.GetComponentInChildren<Camera>();
        
        originalCameraLocalPos = playerCamera.transform.localPosition; // Store original position
    }

    void Update()
    {
        if (Ghost.activeSelf)
        {
            CheckPlayerStare();
            UpdateStareEffects();
        }
    }

    public void StartSequence()
    {
        Ghost.SetActive(true);
        ghostAnimator.Play("GlichtedSitting");
        ghostAudioSource.clip = glitchedBreathing;
        ghostAudioSource.Play();
        glitchBackgroud.clip = glitchedBackground;
        glitchBackgroud.Play();
        scareTrigger.SetActive(false);
        endingTrigger.SetActive(true);
    }

    public void OnPlayerExitRoom()
    {
        ghostAudioSource.Stop();
        Destroy(Ghost);
        Destroy(endingTrigger);
        ResetEffects();
        streetLampScareTrigger.SetActive(true); // Activate the street lamp scare trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject == scareTrigger)
            {
                StartSequence();
            }
            else if (gameObject == endingTrigger)
            {
                OnPlayerExitRoom();
            }
        }
    }

    private void CheckPlayerStare()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 20f, ghostLayer))
        {
            Debug.Log("Player looking at ghost");
            if (hit.collider.gameObject == Ghost)
            {
                stareTime += Time.deltaTime;
            }
            else
            {
                stareTime = Mathf.Max(0, stareTime - Time.deltaTime * 2f);
            }
        }
        else
        {
            stareTime = Mathf.Max(0, stareTime - Time.deltaTime * 2f);
        }
    }

    private void UpdateStareEffects()
    {
        // if (vignette == null || playerCamera == null) return;

        // Calculate effect intensity (0 to 1)
        float effectIntensity = Mathf.Clamp01(stareTime / maxStareTime);

        // Vignette with pulse
        float pulse = Mathf.Sin(Time.time * 5f) * 0.1f; // Pulse amplitude
        // vignette.intensity.value = effectIntensity * 0.7f + pulse;

        // Camera shake
        float shakeIntensity = effectIntensity * shakeIntensityMax;
        Vector3 shakeOffset = originalCameraLocalPos + new Vector3(
            Random.Range(-shakeIntensity, shakeIntensity),
            Random.Range(-shakeIntensity, shakeIntensity),
            0f
        );
        playerCamera.transform.localPosition = shakeOffset;

        // Trigger blackout and teleport
        if (stareTime >= maxStareTime)
        {
            StartCoroutine(BlackoutAndTeleport());
        }
    }

    private IEnumerator BlackoutAndTeleport()
    {
        blackoutPanel.gameObject.SetActive(true);
        float fadeTime = 1f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            blackoutPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }
        blackoutPanel.alpha = 1f;
        
        yield return new WaitForSeconds(1.5f);

        playerTransform.position = teleportTarget.position;
        playerTransform.rotation = teleportTarget.rotation;

        float fadeOutTime = 1f;
        float elapsedOut = 0f;

        while (elapsedOut < fadeOutTime)
        {
            elapsedOut += Time.deltaTime;
            blackoutPanel.alpha = Mathf.Lerp(1f, 0f, elapsedOut / fadeOutTime);
            yield return null;
        }
        blackoutPanel.gameObject.SetActive(false);
        
        ResetEffects();
        stareTime = 0f;

        ghostAudioSource.Stop();
        Destroy(Ghost);
        Destroy(endingTrigger);
        streetLampScareTrigger.SetActive(true); // Activate the street lamp scare trigger
    }

    private void ResetEffects()
    {
        // if (vignette != null) vignette.intensity.value = 0f;
        if (playerCamera != null) playerCamera.transform.localPosition = originalCameraLocalPos; // Reset to original position
    }
}