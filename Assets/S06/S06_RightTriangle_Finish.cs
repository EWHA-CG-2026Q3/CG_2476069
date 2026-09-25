using UnityEngine;
using UnityEngine.UI;

// S05_PixelCanvas_Finish
// 목적: Texture2D 기반 픽셀 캔버스를 생성하고, 단색/랜덤/줄무늬/체스판 무늬를 채운다.

public class S06_RightTriangle_Finish : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    [Header("무늬 실습 (줄무늬·체스판 공용)")]
    [SerializeField] private Color TriangleColor = new Color(1f, 1f, 1f, 1f); // 흰색

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();

        // 1. 빈 캔버스(Texture2D) 생성 — 아직 아무 색도 채워지지 않은 상태
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);

        // 2. 픽셀 경계를 흐리지 않게(확대해도 네모난 픽셀 그대로 보이도록)
        canvasTexture.filterMode = FilterMode.Point;

        // 3. 픽셀 채우기 (실습 단계에 따라 아래 호출을 교체)
        // FillBackground(backgroundColor);
        // FillRandom();
        FillRightTriangle(new Vector2(10, 10), 100, 160, TriangleColor);

        // 4. 지금까지의 SetPixel 변경 사항을 실제로 텍스처에 반영
        canvasTexture.Apply();

        // 5. 완성된 텍스처를 화면의 RawImage에 연결
        targetImage.texture = canvasTexture;
    }

    // 참고 예시 ① — 캔버스 전체를 한 가지 색으로 채움 (모든 픽셀이 같은 값)
    private void FillBackground(Color color)
    {
        Debug.Log("FillBackground");
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, color);
            }
        }
    }

    // 참고 예시 ② — 각 픽셀을 독립적으로 무작위 색으로 채움 (픽셀마다 다른 값, 조건은 없음)
    private void FillRandom()
    {
        Debug.Log("FillRandom");
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
                canvasTexture.SetPixel(x, y, randomColor);
            }
        }
    }

    // 직각삼각형 그리기
    private void FillRightTriangle(Vector2 corner, float legX, float legY, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                float dx = x - corner.x;
                float dy = y - corner.y;
                bool isInside = 
                    dx >= 0 &&                           // 직각삼각형의 코너보다 오른쪽인지
                    dy >= 0 &&                           // 직각삼각형의 코너보다 위쪽인지
                    (dx / legX) + (dy / legY) <= 1f;     // 빗변을 넘지 않았는지. 직선 방정식(x/a + y/b = 1)에서 도출. S05의 (x/size + y/size) % 2 같은 단순 산술 조건
                if (isInside)
                {
                    canvasTexture.SetPixel(x, y, color);
                }
            }
        }
    }

}