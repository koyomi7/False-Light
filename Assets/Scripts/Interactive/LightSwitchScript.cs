using System;
using System.Collections;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour, IInteractable {
    [Header("Light Settings")]
    [SerializeField] bool startsOn = false;

    [Header("Ceiling Fan")]
    [SerializeField] bool isCeilingFan = false;
    [SerializeField] float ceilingFanAcceleration = 0f;
    [SerializeField] float ceilingFanDrag = 0f;
    const float CEILING_FAN_MAX_VELOCITY = 130f;
    float ceilingFanVelocity = 0f;

    [Header("Random Behavior")]
    [SerializeField] private bool enableRandomEvents = true;
    [SerializeField] private float minTimeBetweenEvents = 50f;
    [SerializeField] private float maxTimeBetweenEvents = 120f;
    [SerializeField] private float eventChance = 0.3f;

    [Header("Flicker Settings")]
    [SerializeField] private bool canFlicker = true;
    [SerializeField] private float flickerDuration = 1f;
    [SerializeField] private int flickerCount = 3;

    [Header("References")]
    [SerializeField] GameObject buttonObject;
    [SerializeField] GameObject lightObject;
    [SerializeField] GameObject[] lampObject;
    [SerializeField] Material[] stateMaterial; // inactive = 0, active = 1
    [SerializeField] AudioClip[] toggleOnSound;
    [SerializeField] AudioClip[] toggleOffSound;

    MeshRenderer[] meshRenderer;
    Material[] materials;
    AudioSource audioSource;
    const string activeMaterial = "EmissiveWarm";
    int i = 0; // index of material emission state in materials array
    bool isFlickering = false;
    bool hasLightSwitch;
    [HideInInspector] public bool state; // inactive = false, active = true

    // Glow-in-the-dark light switches
    Material lightSwitchMat;
    Material buttonMat;
    Color lightSwitchEmissionColor;
    Color buttonEmissionColor;

    void Start()
    {
        hasLightSwitch = buttonObject != null;

        // Glow-in-the-dark light switches
        lightSwitchMat = GetComponent<Renderer>().material;
        if (hasLightSwitch) buttonMat = buttonObject.GetComponent<Renderer>().material;
        lightSwitchEmissionColor = new Color(0.05f, 0.05f, 0.05f);
        buttonEmissionColor = new Color(0.8874815f, 1.276985f, 0.8587812f);

        // Builds the materials array to access the emission state
        meshRenderer = new MeshRenderer[lampObject.Length];
        for (int _i = 0; _i < lampObject.Length; _i++)
            meshRenderer[_i] = lampObject[_i].GetComponent<MeshRenderer>();
        materials = meshRenderer[0].materials;

        // Gets the index of the material emission state
        for (; i < materials.Length; i++)
            if (materials[i].name.Contains(activeMaterial)) break;
        
        audioSource = transform.GetComponent<AudioSource>();

        // Sets light state
        SetLightState(startsOn, true);

        if (enableRandomEvents) StartCoroutine(RandomEventCoroutine());
    }

    void Update() {
        // Rotates ceiling fan
        if (isCeilingFan)
        {
            for (int i = 0; i < lampObject.Length; i++)
                lampObject[i].transform.Rotate(0, ceilingFanVelocity * Time.deltaTime, 0);
            ceilingFanVelocity = lightObject.activeSelf ? Mathf.Clamp(ceilingFanVelocity + ceilingFanAcceleration * Time.deltaTime, 0, CEILING_FAN_MAX_VELOCITY) : ceilingFanVelocity = ceilingFanVelocity * (1 - ceilingFanDrag * Time.deltaTime);
        }
    }

    private void SetLightState(bool _state, bool start = false)
    {
        state = _state;
        lightObject.SetActive(_state);
        if (hasLightSwitch) buttonObject.GetComponent<Animator>().Play(_state ? "switchOn" : "switchOff");
        materials[i] = _state ? stateMaterial[1] : stateMaterial[0];
        audioSource.clip = _state ?
            toggleOnSound[UnityEngine.Random.Range(0, toggleOnSound.Length)] :
            toggleOffSound[UnityEngine.Random.Range(0, toggleOffSound.Length)];

        for (int _i = 0; _i < meshRenderer.Length; _i++)
            meshRenderer[_i].materials = materials;

        if (!start) audioSource.Play();

        if (!_state)
        {
            Debug.Log("Off");
            lightSwitchMat.SetColor("_EmissionColor", lightSwitchEmissionColor);
            if (hasLightSwitch) buttonMat.SetColor("_EmissionColor", buttonEmissionColor);
        }
        else
        {
            lightSwitchMat.SetColor("_EmissionColor", Color.black);
            if (hasLightSwitch) buttonMat.SetColor("_EmissionColor", Color.black);
            Debug.Log("On");
        }
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