using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class S07_DepthTest_DepthVisualizer : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    [SerializeField] private Vector3 vertexA1 = new Vector3(100, 180, 0.3f);
    [SerializeField] private Vector3 vertexB1 = new Vector3(60, 80, 0.3f);
    [SerializeField] private Vector3 vertexC1 = new Vector3(180, 80, 0.3f);
    [SerializeField] private Color color1 = new Color(1f, 0.4f, 0.2f, 1f);

    [SerializeField] private Vector3 vertexA2 = new Vector3(150, 200, 0.2f);
    [SerializeField] private Vector3 vertexB2 = new Vector3(90, 60, 0.8f);
    [SerializeField] private Vector3 vertexC2 = new Vector3(220, 60, 0.8f);
    [SerializeField] private Color color2 = new Color(0.2f, 0.5f, 1f, 1f);

    [SerializeField] private RawImage colorTargetImage;
    [SerializeField] private RawImage depthVisualizerImage;

    private Texture2D canvasTexture;
    private Texture2D depthVisTexture;
    private float[,] depthBuffer;

    void OnEnable() { RedrawAll(); }
    void OnValidate() { RedrawAll(); }

    private void RedrawAll()
    {
        if (colorTargetImage == null || depthVisualizerImage == null) return;

        if (canvasTexture == null || canvasTexture.width != canvasWidth || canvasTexture.height != canvasHeight)
        {
            canvasTexture = new Texture2D(canvasWidth, canvasHeight);
            canvasTexture.filterMode = FilterMode.Point;
        }

        depthBuffer = new float[canvasWidth, canvasHeight];
        for (int x = 0; x < canvasWidth; x++)
            for (int y = 0; y < canvasHeight; y++)
                depthBuffer[x, y] = float.MaxValue;

        DrawTriangle(vertexA2, vertexB2, vertexC2, color2);
        DrawTriangle(vertexA1, vertexB1, vertexC1, color1);

        canvasTexture.Apply();
        colorTargetImage.texture = canvasTexture;

        VisualizeDepthBuffer();
    }

    private bool GetBarycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float w1, out float w2, out float w3)
    {
        float denom = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        w1 = (p.x * (b.y - c.y) + b.x * (c.y - p.y) + c.x * (p.y - b.y)) / denom;
        w2 = (a.x * (p.y - c.y) + p.x * (c.y - a.y) + c.x * (a.y - p.y)) / denom;
        w3 = 1f - w1 - w2;
        return w1 >= 0f && w2 >= 0f && w3 >= 0f;
    }

    private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        Vector2 a2 = new Vector2(a.x, a.y);
        Vector2 b2 = new Vector2(b.x, b.y);
        Vector2 c2 = new Vector2(c.x, c.y);

        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 pixelCenter = new Vector2(x + 0.5f, y + 0.5f);
                float w1, w2, w3;
                bool isInside = GetBarycentric(pixelCenter, a2, b2, c2, out w1, out w2, out w3);

                if (isInside)
                {
                    float interpolatedZ = w1 * a.z + w2 * b.z + w3 * c.z;
                    if (interpolatedZ < depthBuffer[x, y])
                    {
                        canvasTexture.SetPixel(x, y, color);
                        depthBuffer[x, y] = interpolatedZ;
                    }
                }
            }
        }
    }

    private void VisualizeDepthBuffer()
    {
        if (depthVisTexture == null || depthVisTexture.width != canvasWidth || depthVisTexture.height != canvasHeight)
        {
            depthVisTexture = new Texture2D(canvasWidth, canvasHeight);
            depthVisTexture.filterMode = FilterMode.Point;
        }

        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                float z = depthBuffer[x, y];
                float brightness = (z == float.MaxValue) ? 0f : 1f - Mathf.InverseLerp(0.2f, 0.9f, z);
                depthVisTexture.SetPixel(x, y, new Color(brightness, brightness, brightness, 1f));
            }
        }
        depthVisTexture.Apply();
        depthVisualizerImage.texture = depthVisTexture;
    }
}