using UnityEngine;

public class S08_DirectTransform : MonoBehaviour
{
    public enum DemoMode
    {
        Translation,
        Scale,
        RotationNaive,
        Rotate,
        TranslateThenRotate,
        RotateThenTranslate,
        TranslateThenScale,
        ScaleThenTranslate,

    }

    [SerializeField] DemoMode demoMode = DemoMode.Translation;
    [SerializeField] Vector3 targetTranslation = new Vector3(3f, 0f, 0f);
    [SerializeField] Vector3 targetScale = new Vector3(2f, 1f, 1f);
    [SerializeField] float targetAngle = 90f;

    DiamondMesh diamondMesh;

    void Awake()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time, 1f);
        Vector3 translation = Vector3.Lerp(Vector3.zero, targetTranslation, t);
        Vector3 scale = Vector3.Lerp(Vector3.one, targetScale, t);
        float angle = Mathf.Lerp(0f, targetAngle, t);
        Vector3[] baseVertices = diamondMesh.BaseVertices;

        Vector3[] verts;
        switch (demoMode)
        {
            case DemoMode.Translation:
                verts = ApplyTranslation(baseVertices, translation);
                break;
            case DemoMode.Scale:
                verts = ApplyScale(baseVertices, scale);
                break;
            case DemoMode.RotationNaive:
                verts = ApplyRotationNaive(baseVertices, angle);
                break;
            case DemoMode.Rotate:
                verts = ApplyRotation(baseVertices, angle);
                break;
            case DemoMode.TranslateThenRotate:
                verts = ApplyTranslation(baseVertices, translation);
                verts = ApplyRotation(verts, angle);      // 같은 함수, 순서만 뒤에
                break;
            case DemoMode.RotateThenTranslate:
                verts = ApplyRotation(baseVertices, angle);
                verts = ApplyTranslation(verts, translation); // 같은 함수, 순서만 앞에
                break;
            case DemoMode.TranslateThenScale:
                verts = ApplyTranslation(baseVertices, translation);
                verts = ApplyScale(verts, scale);
                break;
            case DemoMode.ScaleThenTranslate:
                verts = ApplyScale(baseVertices, scale);
                verts = ApplyTranslation(verts, translation);
                break;
            default:
                verts = baseVertices;
                break;
        }
        diamondMesh.SetVertices(verts);
    }

    public Vector3[] ApplyTranslation(Vector3[] baseVertices, Vector3 t)
    {
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = baseVertices[i] + t;
        return verts;
    }

    public Vector3[] ApplyScale(Vector3[] baseVertices, Vector3 s)
    {
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            verts[i] = new Vector3(
                baseVertices[i].x * s.x,
                baseVertices[i].y * s.y,
                baseVertices[i].z * s.z);
        }
        return verts;
    }

    public Vector3[] ApplyRotationNaive(Vector3[] baseVertices, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            verts[i] = new Vector3(v.x * c, v.y * s, v.z);
        }
        return verts;
    }

    public Vector3[] ApplyRotation(Vector3[] baseVertices, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float newX = v.x * c - v.y * s;
            float newY = v.x * s + v.y * c;
            verts[i] = new Vector3(newX, newY, v.z);
        }
        return verts;
    }


}