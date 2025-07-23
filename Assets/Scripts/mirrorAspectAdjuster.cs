using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MirrorAspectAdjuster : MonoBehaviour {
    public Transform mirrorPlane;
    private Camera mirrorCamera;

    void Start() {
        mirrorCamera = GetComponent<Camera>();
        AdjustMirrorSettings();
    }

    void AdjustMirrorSettings() {
        if (mirrorPlane != null && mirrorCamera != null) {
            // Calculate aspect ratio from plane scale
            Vector3 scale = mirrorPlane.lossyScale; // Use lossyScale for world scale
            float aspect = Mathf.Abs(scale.x / scale.y);
            mirrorCamera.aspect = aspect;

            // Force camera to reset projection matrix
            mirrorCamera.ResetProjectionMatrix();
        }
    }
}