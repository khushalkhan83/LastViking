using UnityEngine;

public class WaterReflectionController : MonoBehaviour
{
    public int PropertyIDReflectionTexture { get; } = Shader.PropertyToID("_ReflectionTex");

    #region Data
#pragma warning disable 0649

    [SerializeField] private bool _isCulling;
    [SerializeField] private bool _isNotUsePixelLight;
    [SerializeField] public LayerMask _renderLayerMasks;
    [SerializeField] private Camera _cameraBackground;
    [SerializeField] private Renderer[] _renderers;

#pragma warning restore 0649
    #endregion

    public bool IsCulling => _isCulling;
    public bool IsNotUsePixelLight => _isNotUsePixelLight;
    public LayerMask RenderLayerMasks => _renderLayerMasks;
    public Camera CameraBackground => _cameraBackground;
    public Renderer[] Renderers => _renderers;

    public DepthTextureMode DepthTextureMode { get; private set; }
    public Camera CameraRender { get; private set; }
    public Camera CameraTarget { get; private set; }
    public RenderTexture RenderTexture { get; private set; }

    static public int LayerAllNotWater { get; } = ~8;
    static public bool IsRendering { get; private set; }

    public void SetRenderTextureSize(int size)
    {
        var isValidSize = size > 0;
        var isDestroyOld = RenderTexture != null && (RenderTexture.width != size || !isValidSize);
        var isCreateNew = isValidSize && (RenderTexture == null || (RenderTexture != null && RenderTexture.width != size));

        if (isDestroyOld)
        {
            DestroyImmediate(RenderTexture);
            RenderTexture = null;
        }

        if (isCreateNew)
        {
            RenderTexture = new RenderTexture(size, size, 1)
            {
                name = string.Empty,
                isPowerOfTwo = true,
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }

    //

    private void OnEnable()
    {
        CreateCameraRender();
    }

    private void OnDisable()
    {
        if (RenderTexture != null)
        {
            DestroyImmediate(RenderTexture);
            RenderTexture = null;
        }

        if (CameraRender != null)
        {
            DestroyImmediate(CameraRender.gameObject);
            CameraRender = null;
        }
    }

    private bool IsRenderersEnabled()
    {
        foreach (var renderer in Renderers)
        {
            if (!renderer.enabled) return false;
        }
        return true;
    }

    private bool HasRenderersShared()
    {
        foreach (var renderer in Renderers)
        {
            if (!renderer.sharedMaterial) return false;
        }
        return true;
    }

    public void OnWillRenderObject()
    {
        var cam = Camera.current;

        if
        (
            !enabled
            || !RenderTexture
            || (Renderers != null)
            || !IsRenderersEnabled()
            || !HasRenderersShared()
            || !cam
            || !CameraTarget
            || cam != CameraTarget
            || IsRendering // stop recursive render when many different reflection objects
        )
        {
            return;
        }

        IsRendering = true;

        var oldPixelLightCount = QualitySettings.pixelLightCount;

        if (IsNotUsePixelLight)
        {
            QualitySettings.pixelLightCount = 0;
        }

        Render(CameraBackground, RenderTexture, transform.position, Vector3.up);
        Render(CameraTarget, RenderTexture, transform.position, Vector3.up);

        foreach (var renderer in Renderers)
        {
            UpdateRenderer(renderer);
        }

        if (IsNotUsePixelLight)
        {
            QualitySettings.pixelLightCount = oldPixelLightCount;
        }

        IsRendering = false;
    }

    private void UpdateRenderer(Renderer renderer)
    {
        if (renderer.material.HasProperty(PropertyIDReflectionTexture))
        {
            renderer.material.SetTexture(PropertyIDReflectionTexture, RenderTexture);
        }
    }

    private void CreateCameraRender()
    {
        CameraRender = new GameObject()
        {
            hideFlags = HideFlags.HideAndDontSave,
            name = string.Empty
        }.AddComponent<Camera>();

        CameraRender.enabled = false;
        CameraRender.transform.position = transform.position;
        CameraRender.transform.rotation = transform.rotation;
    }

    public void SetTargetCamera(Camera camera) => CameraTarget = camera;

    private void Render(Camera cam, RenderTexture renderTexture, Vector3 pos, Vector3 normal)
    {
        var dot = -Vector3.Dot(normal, pos);
        var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, dot);
        var reflection = CalculateReflectionMatrix(reflectionPlane);
        var oldpos = cam.transform.position;
        var newpos = reflection.MultiplyPoint(oldpos);

        CameraRender.aspect = cam.aspect;
        CameraRender.clearFlags = cam.clearFlags;
        CameraRender.cullingMask = RenderLayerMasks.value & cam.cullingMask & LayerAllNotWater;
        CameraRender.backgroundColor = cam.backgroundColor;
        CameraRender.farClipPlane = cam.farClipPlane;
        CameraRender.nearClipPlane = cam.nearClipPlane;
        CameraRender.orthographic = cam.orthographic;
        CameraRender.fieldOfView = cam.fieldOfView;
        CameraRender.orthographicSize = cam.orthographicSize;
        CameraRender.useOcclusionCulling = IsCulling;
        CameraRender.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
        CameraRender.projectionMatrix = GetCalculateObliqueMatrix(cam.projectionMatrix, CameraSpacePlane(CameraRender, pos, normal, 1f));
        CameraRender.targetTexture = renderTexture;
        CameraRender.transform.eulerAngles = new Vector3(0f, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        CameraRender.transform.position = newpos;
        GL.invertCulling = true;

        CameraRender.Render();

        GL.invertCulling = false;
        CameraRender.transform.position = oldpos;
    }

    private static float Sign(float value)//Refactor use alternative as Mathf.Sign?
    {
        if (value > 0f)
        {
            return 1f;
        }

        if (value < 0f)
        {
            return -1f;
        }

        return 0f;
    }

    private Vector4 CameraSpacePlane(Camera camera, Vector3 position, Vector3 normal, float sideSign)
    {
        var matrix = camera.worldToCameraMatrix;
        var cpos = matrix.MultiplyPoint(position);
        var cnormal = matrix.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    static private Matrix4x4 GetCalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
    {
        var q = projection.inverse * new Vector4(Sign(clipPlane.x), Sign(clipPlane.y), 1f, 1f);
        var c = clipPlane * 2f / Vector4.Dot(clipPlane, q);
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];

        return projection;
    }

    private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        return new Matrix4x4()
        {
            m00 = 1f - 2f * plane[0] * plane[0],
            m01 = -2f * plane[0] * plane[1],
            m02 = -2f * plane[0] * plane[2],
            m03 = -2f * plane[3] * plane[0],

            m10 = -2f * plane[1] * plane[0],
            m11 = 1f - 2f * plane[1] * plane[1],
            m12 = -2f * plane[1] * plane[2],
            m13 = -2f * plane[3] * plane[1],

            m20 = -2f * plane[2] * plane[0],
            m21 = -2f * plane[2] * plane[1],
            m22 = 1f - 2f * plane[2] * plane[2],
            m23 = -2f * plane[3] * plane[2],

            //m30 = 0F,
            //m31 = 0F,
            //m32 = 0F,
            m33 = 1f
        };
    }
}
