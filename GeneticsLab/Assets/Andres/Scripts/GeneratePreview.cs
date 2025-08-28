#if UNITY_EDITOR
// GeneratePreview.cs (Editor-only)
// Renders a thumbnail from a temporary camera that is created per-capture and destroyed afterward.
// Includes gizmos for capture area, a resolution dropdown, layer selection,
// and TriInspector buttons for GenerateThumbnail + Align To Scene View.

using System;
using System.IO;
using UnityEngine;
using UnityEditor;

// #if TRI_INSPECTOR || TRI_INSPECTOR_FREE
// using TriInspector;
// #endif
using EasyButtons;

public class GeneratePreview : MonoBehaviour
{
    [Header("Source Camera (settings are copied to a temporary camera at capture time)")]
    [Tooltip("Camera whose transform and lens settings are copied when generating the thumbnail.")]
    public Camera sourceCamera;

    [Header("Layers")]
    [Tooltip("Only these layers will be rendered for the thumbnail.")]
    public LayerMask thumbnailLayers = ~0;

    [Header("Resolution")]
    public ResolutionPreset preset = ResolutionPreset._1024;
    [Tooltip("Used when Preset = Custom.")]
    public Vector2Int customResolution = new Vector2Int(1920, 1080);

    [Header("Saving")]
    [Tooltip("PNG will be saved here when using auto-save. Relative to the project folder (inside Assets).")]
    public string defaultSaveFolder = "Assets/Thumbnails";

    [Tooltip("If true, the PNG will be saved automatically to Default Save Folder with a unique name. If false, a save panel will prompt you.")]
    public bool autoSaveToFolder = true;

    [Header("Gizmo Preview")]
    [Tooltip("Distance in front of the source camera at which to draw the capture rectangle gizmo.")]
    [Min(0.01f)]
    public float gizmoDepth = 1.0f;

    [Tooltip("Draw the full camera frustum as a wireframe as well.")]
    public bool drawFrustum = true;

    public enum ResolutionPreset
    {
        _256   = 256,
        _512   = 512,
        _1024  = 1024,
        _2048  = 2048,
        _4096  = 4096,
        Custom = -1
    }

    private Vector2Int GetTargetResolution()
    {
        if (preset == ResolutionPreset.Custom)
        {
            var w = Mathf.Max(8, customResolution.x);
            var h = Mathf.Max(8, customResolution.y);
            return new Vector2Int(w, h);
        }

        int size = (int)preset;

        // Honor camera aspect if possible; otherwise square.
        float aspect = (sourceCamera != null && sourceCamera.pixelHeight > 0)
            ? sourceCamera.aspect
            : 1.0f;

        int width = size;
        int height = Mathf.Max(8, Mathf.RoundToInt(size / Mathf.Max(0.01f, aspect)));
        return new Vector2Int(width, height);
    }

// #if TRI_INSPECTOR || TRI_INSPECTOR_FREE
    [Button("Generate Thumbnail")]
// #endif
//     [ContextMenu("Generate Thumbnail")]
    public void GenerateThumbnail()
    {
        if (sourceCamera == null)
        {
            Debug.LogError("[GeneratePreview] Source Camera is not assigned.");
            return;
        }

        Vector2Int res = GetTargetResolution();
        if (res.x <= 0 || res.y <= 0)
        {
            Debug.LogError("[GeneratePreview] Invalid resolution.");
            return;
        }

        // Create a temporary hidden camera that copies settings from the source camera
        var tempGo = new GameObject("~TempThumbnailCamera");
        tempGo.hideFlags = HideFlags.HideAndDontSave;
        var tempCam = tempGo.AddComponent<Camera>();

        try
        {
            CopyCameraSettings(sourceCamera, tempCam);

            // Override culling for the capture
            tempCam.cullingMask = thumbnailLayers;

            var rt = new RenderTexture(res.x, res.y, 24, RenderTextureFormat.ARGB32)
            {
                useMipMap = false,
                autoGenerateMips = false
            };

            var prevActive = RenderTexture.active;
            tempCam.targetTexture = rt;
            tempCam.Render();

            var tex = new Texture2D(res.x, res.y, TextureFormat.RGBA32, false, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, res.x, res.y), 0, 0, false);
            tex.Apply(false, false);
            RenderTexture.active = prevActive;

            byte[] pngBytes = tex.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(tex);

            SavePngInEditor(pngBytes, res);

            // Cleanup
            tempCam.targetTexture = null;
            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (tempGo != null) UnityEngine.Object.DestroyImmediate(tempGo);
        }
    }

// #if TRI_INSPECTOR || TRI_INSPECTOR_FREE
    [Button("Align To Scene View")]
// #endif
//     [ContextMenu("Align Source Camera To Scene View")]
    public void AlignPreviewCameraToSceneView()
    {
        if (sourceCamera == null)
        {
            Debug.LogError("[GeneratePreview] Source Camera is not assigned.");
            return;
        }

        var sv = SceneView.lastActiveSceneView;
        if (sv == null)
        {
            Debug.LogWarning("[GeneratePreview] No active Scene View to align from.");
            return;
        }

        // Match transform & lens
        sourceCamera.transform.SetPositionAndRotation(sv.camera.transform.position, sv.camera.transform.rotation);

        sourceCamera.orthographic = sv.orthographic;
        if (sourceCamera.orthographic)
        {
            sourceCamera.orthographicSize = sv.size;
        }
        else
        {
            sourceCamera.fieldOfView = sv.camera.fieldOfView;
        }

        sourceCamera.nearClipPlane = sv.camera.nearClipPlane;
        sourceCamera.farClipPlane  = sv.camera.farClipPlane;

        Debug.Log("[GeneratePreview] Source camera aligned to the Scene View.");
    }

    private static void CopyCameraSettings(Camera from, Camera to)
    {
        // Transform handled externally when we clone; set here too for safety
        to.transform.SetPositionAndRotation(from.transform.position, from.transform.rotation);

        // Projection
        to.orthographic     = from.orthographic;
        to.fieldOfView      = from.fieldOfView;
        to.orthographicSize = from.orthographicSize;
        to.aspect           = from.aspect;

        // Clipping & rendering
        to.nearClipPlane    = from.nearClipPlane;
        to.farClipPlane     = from.farClipPlane;
        to.clearFlags       = from.clearFlags;
        to.backgroundColor  = from.backgroundColor;
        to.allowHDR         = from.allowHDR;
        to.allowMSAA        = from.allowMSAA;
        to.allowDynamicResolution = from.allowDynamicResolution;

#if !UNITY_6000_0_OR_NEWER
        to.forceIntoRenderTexture = from.forceIntoRenderTexture;
#endif

        // Skybox copy (if any)
        var fromSky = from.GetComponent<Skybox>();
        var toSky   = to.GetComponent<Skybox>();
        if (fromSky && fromSky.material)
        {
            if (!toSky) toSky = to.gameObject.AddComponent<Skybox>();
            toSky.material = fromSky.material;
        }
        else if (toSky)
        {
            UnityEngine.Object.DestroyImmediate(toSky);
        }
    }

    private void SavePngInEditor(byte[] pngBytes, Vector2Int res)
    {
        if (pngBytes == null || pngBytes.Length == 0) return;

        string finalPath;
        if (autoSaveToFolder)
        {
            if (string.IsNullOrWhiteSpace(defaultSaveFolder))
                defaultSaveFolder = "Assets/Thumbnails";

            EnsureFolderPath(defaultSaveFolder);

            var filename = $"thumbnail_{gameObject.name}_{DateTime.Now:yyyyMMdd_HHmmss}_{res.x}x{res.y}.png";
            var rawPath = Path.Combine(defaultSaveFolder, filename).Replace("\\", "/");
            finalPath = AssetDatabase.GenerateUniqueAssetPath(rawPath);
        }
        else
        {
            string suggested = $"thumbnail_{gameObject.name}_{res.x}x{res.y}.png";
            finalPath = EditorUtility.SaveFilePanelInProject("Save Thumbnail PNG", suggested, "png", "Choose save location for the thumbnail PNG.");
            if (string.IsNullOrEmpty(finalPath))
            {
                Debug.Log("[GeneratePreview] Save canceled.");
                return;
            }
        }

        File.WriteAllBytes(finalPath, pngBytes);
        AssetDatabase.ImportAsset(finalPath);
        Debug.Log($"[GeneratePreview] Thumbnail saved: {finalPath}");
    }

    private static void EnsureFolderPath(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        var parts = path.Replace("\\", "/").Split('/');
        var root = "Assets";
        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (string.IsNullOrEmpty(part) || part == "Assets") continue;

            var next = $"{root}/{part}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(root, part);
            }
            root = next;
        }
    }

    // -------- Gizmos (drawn from the source camera) --------
    private void OnDrawGizmosSelected()
    {
        if (sourceCamera == null) return;

        if (drawFrustum)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            float fov = sourceCamera.fieldOfView;
            float aspect = sourceCamera.aspect <= 0 ? 1f : sourceCamera.aspect;
            float near = Mathf.Max(0.001f, sourceCamera.nearClipPlane);
            float far  = Mathf.Max(near + 0.001f, sourceCamera.farClipPlane);

            Gizmos.matrix = sourceCamera.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, fov, far, near, aspect);
        }

        DrawCaptureRectAtDepth(sourceCamera, gizmoDepth, new Color(1f, 0.5f, 0f, 0.9f));
    }

    private static void DrawCaptureRectAtDepth(Camera cam, float depth, Color color)
    {
        if (!cam) return;
        if (depth <= 0f) depth = 0.01f;

        float halfHeight;
        if (cam.orthographic)
        {
            halfHeight = cam.orthographicSize;
        }
        else
        {
            float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
            halfHeight = Mathf.Tan(fovRad * 0.5f) * depth;
        }
        float halfWidth = halfHeight * Mathf.Max(0.01f, cam.aspect);

        var t = cam.transform;
        Vector3 center = t.position + t.forward * depth;
        Vector3 right  = t.right * halfWidth;
        Vector3 up     = t.up * halfHeight;

        Vector3 tl = center + up - right;
        Vector3 tr = center + up + right;
        Vector3 br = center - up + right;
        Vector3 bl = center - up - right;

        Gizmos.color = color;
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
        Gizmos.DrawLine(bl, tl);

        Gizmos.DrawLine(t.position, tl);
        Gizmos.DrawLine(t.position, tr);
        Gizmos.DrawLine(t.position, br);
        Gizmos.DrawLine(t.position, bl);
    }
}
#endif
