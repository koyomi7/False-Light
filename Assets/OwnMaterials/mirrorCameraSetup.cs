using UnityEngine;

public class MirrorCameraSetup : MonoBehaviour
{
    public RenderTexture mirrorRenderTexture;

    void Start()
    {
        // Create render texture with alpha channel support
        mirrorRenderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
        mirrorRenderTexture.Create();

        // Configure camera
        Camera cam = GetComponent<Camera>();
        cam.targetTexture = mirrorRenderTexture;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // Transparent background
    }
}
