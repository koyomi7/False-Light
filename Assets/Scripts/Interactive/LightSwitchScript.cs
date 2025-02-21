using System;
using System.Collections;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour, IInteractable {
    [SerializeField] private GameObject lightSwitchObject;
    [SerializeField] private GameObject lightObject;
    [SerializeField] private GameObject[] lampObject;
    [SerializeField] private Material[] stateMaterial; // inactive = 0, active = 1
    [SerializeField] private AudioClip[] toggleOnSound;
    [SerializeField] private AudioClip[] toggleOffSound;

    [Header("Random Behavior")]
    [SerializeField] private bool enableRandomEvents = true;
    [SerializeField] private float minTimeBetweenEvents = 50f;
    [SerializeField] private float maxTimeBetweenEvents = 120f;
    [SerializeField] private float eventChance = 0.3f;

    [Header("Flicker Settings")]
    [SerializeField] private bool canFlicker = true;
    [SerializeField] private float flickerDuration = 1f;
    [SerializeField] private int flickerCount = 3;
    [SerializeField] private bool isCeilingFan = false;

    private MeshRenderer[] meshRenderer;
    private Material[] materials;
    private AudioSource audioSource;
    private const string activeMaterial = "EmissiveWarm";
    private int i = 0; // index of material emission state in materials array
    private bool isFlickering = false;
    private bool hasLightSwitch;
    private const float CEILING_FAN_MAX_VELOCITY = 130f;
    private float ceilingFanVelocity = 0f;
    [SerializeField] private float ceilingFanAcceleration = 0f;
    [SerializeField] private float ceilingFanDrag = 0f;

    void Start() {
        meshRenderer = new MeshRenderer[lampObject.Length];
        for (int _i = 0; _i < lampObject.Length; _i++)
            meshRenderer[_i] = lampObject[_i].GetComponent<MeshRenderer>();
        materials = meshRenderer[0].materials;

        for (; i < materials.Length; i++)
            if (materials[i].name.Contains(activeMaterial)) break;
        
        audioSource = transform.GetComponent<AudioSource>();

        hasLightSwitch = lightSwitchObject != null;

        // Starts off
        SetLightState(false, true);

        if (enableRandomEvents)
        {
            StartCoroutine(RandomEventCoroutine());
        }
    }

    void Update() {
        if (isCeilingFan) {
            // Rotate ceiling fan
            for (int i = 0; i < lampObject.Length; i++) {
                lampObject[i].transform.Rotate(0, ceilingFanVelocity * Time.deltaTime, 0);
            }
            if (lightObject.activeSelf)
                ceilingFanVelocity = Mathf.Clamp(ceilingFanVelocity + ceilingFanAcceleration * Time.deltaTime, 0, CEILING_FAN_MAX_VELOCITY);
            else ceilingFanVelocity = ceilingFanVelocity * (1 - ceilingFanDrag * Time.deltaTime);
        }
    }

    private void SetLightState(bool state, bool start=false) {
        lightObject.SetActive(state);
        if (hasLightSwitch) lightSwitchObject.GetComponent<Animator>().Play(state ? "switchOn" : "switchOff");
        materials[i] = state ? stateMaterial[1] : stateMaterial[0];
        audioSource.clip = state ? 
            toggleOnSound[UnityEngine.Random.Range(0, toggleOnSound.Length)] : 
            toggleOffSound[UnityEngine.Random.Range(0, toggleOffSound.Length)];

        for (int _i = 0; _i < meshRenderer.Length; _i++)
            meshRenderer[_i].materials = materials;
        
        if (!start) audioSource.Play();
    }

    public void Interact() {
        if (!isFlickering)
        {
            Debug.Log("Toggled light switch");
            SetLightState(!lightObject.activeSelf);
        }
    }

    private IEnumerator RandomEventCoroutine()
    {
        while (enableRandomEvents)
        {
            float waitTime = UnityEngine.Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
            yield return new WaitForSeconds(waitTime);

            if (UnityEngine.Random.value < eventChance && lightObject.activeSelf)
            {
                if (UnityEngine.Random.value < 0.5f && canFlicker)
                {
                    StartCoroutine(FlickerEffect());
                }
                else
                {
                    SetLightState(false);
                    // Optionally turn it back on after a delay
                    StartCoroutine(TurnOnAfterDelay(UnityEngine.Random.Range(2f, 5f)));
                }
            }
        }
    }

    private IEnumerator FlickerEffect()
    {
        if (isFlickering) yield break;

        isFlickering = true;
        float flickerInterval = flickerDuration / (flickerCount * 2);

        for (int j = 0; j < flickerCount; j++)
        {
            SetLightState(false);
            yield return new WaitForSeconds(flickerInterval);
            SetLightState(true);
            yield return new WaitForSeconds(flickerInterval);
        }

        isFlickering = false;
    }

    private IEnumerator TurnOnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isFlickering)
        {
            SetLightState(true);
        }
    }

    // Public methods to control random behavior
    public void EnableRandomEvents(bool enable)
    {
        enableRandomEvents = enable;
        if (enable && !IsInvoking("RandomEventCoroutine"))
        {
            StartCoroutine(RandomEventCoroutine());
        }
    }

    public void TriggerFlicker()
    {
        if (!isFlickering && lightObject.activeSelf)
        {
            StartCoroutine(FlickerEffect());
        }
    }
}