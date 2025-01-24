using System.Collections;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour {
    [SerializeField] private GameObject lightObject;
    [SerializeField] private GameObject[] lampObject;
    [SerializeField] private Material[] stateMaterial; // inactive = 0, active = 1
    private MeshRenderer[] meshRenderer;
    private Material[] materials;
    private const string activeMaterial = "EmissiveWarm";
    private int i = 0; // index of material emission state in materials array

    void Start() {
        meshRenderer = new MeshRenderer[lampObject.Length];
        for (int _i = 0; _i < lampObject.Length; _i++)
            meshRenderer[_i] = lampObject[_i].GetComponent<MeshRenderer>();
        materials = meshRenderer[0].materials; // only need one of these assuming all prefabs are identical

        // Gets the index of the material that needs to be changed
        for (; i < materials.Length; i++)
            if (materials[i].name.Contains(activeMaterial)) break;
        
        // test function to toggle switch every 3 seconds
        StartCoroutine(TestLightSwitch());
    }

    IEnumerator TestLightSwitch() {
        Debug.Log("Toggled light switch");
        lightObject.SetActive(!lightObject.activeSelf);
        if (lightObject.activeSelf) {
            transform.GetComponent<Animator>().Play("switchOn");
            materials[i] = stateMaterial[1];
        }
        else {
            transform.GetComponent<Animator>().Play("switchOff");
            materials[i] = stateMaterial[0];
        }
        for (int _i = 0; _i < meshRenderer.Length; _i++)
            meshRenderer[_i].materials = materials;

        yield return new WaitForSeconds(3);
        StartCoroutine(TestLightSwitch());
    }
}
