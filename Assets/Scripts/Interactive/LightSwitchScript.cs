using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour, IInteractable {
    [Header("Light Settings")]
    [SerializeField] bool startsOn = false;
    
    [Header("Flicker")]
    [SerializeField] bool isFlickering = false;
    [SerializeField] float flickerSpeed = 1f;
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float maxIntensity = 1f;
    Light _light;
    float originalIntensity;

    [Header("Ceiling Fan")]
    [SerializeField] bool isCeilingFan = false;
    [SerializeField] float ceilingFanAcceleration = 0f;
    [SerializeField] float ceilingFanDrag = 0f;
    const float maxCeilingFanVelocity = 130f;
    float ceilingFanVelocity = 0f;

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

        _light = lightObject.GetComponent<Light>();
        originalIntensity = _light.intensity;

        // Sets light state
        SetLightState(startsOn, true);
    }

    void Update()
    {
        HandleCeilingFan();
        HandleFlickering();
    }

    void HandleCeilingFan()
    {
        if (!isCeilingFan) return;

        // Rotates ceiling fan
        for (int i = 0; i < lampObject.Length; i++)
            lampObject[i].transform.Rotate(0, ceilingFanVelocity * Time.deltaTime, 0);
        ceilingFanVelocity = lightObject.activeSelf
        ? Mathf.Clamp(ceilingFanVelocity + ceilingFanAcceleration * Time.deltaTime, 0, maxCeilingFanVelocity)
        : ceilingFanVelocity * (1 - ceilingFanDrag * Time.deltaTime);
    }

    void HandleFlickering()
    {
        if (!isFlickering) return;
        float t = (Mathf.Sin(Time.time * flickerSpeed * Mathf.PI) + 1f) * 0.5f; // 0-1 range
        _light.intensity = Mathf.Lerp(originalIntensity * minIntensity, originalIntensity * maxIntensity, t);
    }

    private void SetLightState(bool _state, bool start = false)
    {
        state = _state;
        lightObject.SetActive(_state);
        if (hasLightSwitch) buttonObject.GetComponent<Animator>().Play(_state ? "switchOn" : "switchOff");
        materials[i] = _state ? stateMaterial[1] : stateMaterial[0];
        audioSource.clip = _state
        ? toggleOnSound[UnityEngine.Random.Range(0, toggleOnSound.Length)]
        : toggleOffSound[UnityEngine.Random.Range(0, toggleOffSound.Length)];

        for (int _i = 0; _i < meshRenderer.Length; _i++)
            meshRenderer[_i].materials = materials;

        if (!start) audioSource.Play();

        // Glow-in-the-dark light switch
        Color targetColor = _state ? Color.black : lightSwitchEmissionColor;
        lightSwitchMat.SetColor("_EmissionColor", targetColor);
        if (hasLightSwitch)
            buttonMat.SetColor("_EmissionColor", _state ? Color.black : buttonEmissionColor);
    }

    public void Interact()
    {
        SetLightState(!lightObject.activeSelf);
        Debug.Log(state ? $"Player turned on light switch" : $"Player turned off light switch");
    }
}