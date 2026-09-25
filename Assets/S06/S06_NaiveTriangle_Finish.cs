using UnityEngine;
using UnityEngine.UI;

public class S06_NaiveTriangle : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;
    [SerializeField] private Vector2 vertexA = new Vector2(128, 200);
    [SerializeField] private Vector2 vertexB = new Vector2(60, 60);
    [SerializeField] private Vector2 vertexC = new Vector2(200, 60);
    [SerializeField] private Color fillColor = new Color(1f, 0.6f, 0.2f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        FillTriangleNaive(vertexA, vertexB, vertexC, fillColor);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void FillTriangleNaive(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);

                if (IsOnInsideOfEdge(p, a, b, c) &&
                    IsOnInsideOfEdge(p, b, c, a) &&
                    IsOnInsideOfEdge(p, c, a, b))
                {
                    canvasTexture.SetPixel(x, y, color);
                }
            }
        }
    }

    // p가 변(p1→p2) 안쪽에 있는지, 반대쪽 꼭짓점(reference)을 기준으로 판단
    private bool IsOnInsideOfEdge(Vector2 p, Vector2 p1, Vector2 p2, Vector2 reference)
    {
        if (Mathf.Approximately(p2.x, p1.x))
        {
            // 수직선 — 기울기(m)를 구할 수 없음(분모가 0), 별도 처리 필요
            // 수직선을 기준으로 "왼쪽/오른쪽"만 비교하면 됨(위/아래 개념이 없으므로)

            bool referenceIsRight = reference.x > p1.x;  // reference가 이 수직선보다 오른쪽에 있는지
            bool pIsRight = p.x > p1.x;                  // p가 이 수직선보다 오른쪽에 있는지

            // 둘이 같은 쪽(둘 다 오른쪽 또는 둘 다 왼쪽)이면 p는 이 변의 안쪽
            return referenceIsRight == pIsRight;
        }
        else
        {
            // 수직선이 아닌 일반적인 경우 — 직선의 기울기(m)와 y절편(b)을 구함
            float m = (p2.y - p1.y) / (p2.x - p1.x);  // 기울기
            float b = p1.y - m * p1.x;                 // y절편 (y = mx + b에서 x=p1.x일 때 y=p1.y가 되도록 역산)

            // reference와 p가 각각 이 직선보다 위에 있는지 아래에 있는지 확인
            bool referenceIsAbove = reference.y > (m * reference.x + b);  // reference가 직선 위에 있는지
            bool pIsAbove = p.y > (m * p.x + b);                          // p가 직선 위에 있는지

            // 둘이 같은 쪽(둘 다 위 또는 둘 다 아래)이면 p는 이 변의 안쪽
            return referenceIsAbove == pIsAbove;
        }
    }
}