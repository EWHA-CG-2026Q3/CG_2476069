
using UnityEngine;

public class PictureChess : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    [SerializeField] private int gridCount = 8;

    [SerializeField] private Color colorA = new Color(0.8f, 0.02f, 0.05f, 1f);
    [SerializeField] private Color colorB = new Color(0.02f, 0.35f, 0.08f, 1f);
    [SerializeField] private Color backgroundColor = Color.white;

    private Texture2D canvasTexture;
    private Renderer pictureRenderer;

    void Start()
    {
        // 이 스크립트가 붙어 있는 Quad의 Renderer 가져오기
        pictureRenderer = GetComponent<Renderer>();

        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        FillBackground(backgroundColor);
        DrawCheckerboard();

        canvasTexture.Apply();

        // 생성한 체스판을 Quad에 직접 적용
        pictureRenderer.material.mainTexture = canvasTexture;
    }

    private void FillBackground(Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, color);
            }
        }
    }

    private bool GetBarycentric(
        Vector2 p, Vector2 a, Vector2 b, Vector2 c,
        out float w1, out float w2, out float w3)
    {
        float denom = a.x * (b.y - c.y)
                    + b.x * (c.y - a.y)
                    + c.x * (a.y - b.y);

        w1 = (p.x * (b.y - c.y)
            + b.x * (c.y - p.y)
            + c.x * (p.y - b.y)) / denom;

        w2 = (a.x * (p.y - c.y)
            + p.x * (c.y - a.y)
            + c.x * (a.y - p.y)) / denom;

        w3 = 1f - w1 - w2;

        return w1 >= 0f && w2 >= 0f && w3 >= 0f;
    }

    private void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 pixelCenter = new Vector2(x + 0.5f, y + 0.5f);

                float w1, w2, w3;

                if (GetBarycentric(pixelCenter, a, b, c, out w1, out w2, out w3))
                {
                    canvasTexture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawCheckerboard()
    {
        float cellWidth = (float)canvasWidth / gridCount;
        float cellHeight = (float)canvasHeight / gridCount;

        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                Color cellColor = (x + y) % 2 == 0 ? colorA : colorB;

                float x0 = x * cellWidth;
                float x1 = (x + 1) * cellWidth;
                float y0 = y * cellHeight;
                float y1 = (y + 1) * cellHeight;

                Vector2 bottomLeft = new Vector2(x0, y0);
                Vector2 bottomRight = new Vector2(x1, y0);
                Vector2 topLeft = new Vector2(x0, y1);
                Vector2 topRight = new Vector2(x1, y1);

                DrawTriangle(bottomLeft, topLeft, topRight, cellColor);
                DrawTriangle(bottomLeft, topRight, bottomRight, cellColor);
            }
        }
    }
}
