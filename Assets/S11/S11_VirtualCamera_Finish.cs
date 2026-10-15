using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

// 빈 오브젝트(VirtualCamera)에 붙여, 이 오브젝트를 카메라로 삼아 장면을 캔버스에 와이어프레임으로 그림
// 뷰 변환: 카메라를 기준으로 잰 정점 = V × M × 정점, V = R⁻¹ × T⁻¹
// 캔버스에 그리는 규칙: 정투영 (z는 그릴지 말지만 정하고, 위치 계산에는 쓰지 않음)
public class S11_VirtualCamera_Finish : MonoBehaviour
{
    [Header("그릴 오브젝트")]
    public MeshFilter[] targets;                                  // 캔버스에 그릴 오브젝트들의 MeshFilter

    [Header("캔버스에 담을 범위")]
    public float size = 2f;                                       // 캔버스 세로 전체에 위로 size, 아래로 size 유닛을 담음
    public float near = 0.3f;                                     // 이보다 가까운 정점은 그리지 않음
    public float far = 20f;                                       // 이보다 먼 정점은 그리지 않음

    [Header("캔버스")]
    public RawImage canvasImage;                                  // 캔버스를 보여 줄 RawImage (S05처럼 직접 연결)
    public int width = 400;                                       // 캔버스 가로 픽셀 수
    public int height = 300;                                      // 캔버스 세로 픽셀 수
    public Color backgroundColor = Color.black;                   // 캔버스 바탕색
    public Color lineColor = Color.white;                         // 와이어프레임 선 색

    Texture2D canvas;                                             // 픽셀을 직접 칠하는 텍스처
    Color[] clearPixels;                                          // 캔버스를 지울 때 한 번에 채울 바탕색 배열

    void OnEnable()
    {
        CreateCanvas();                                           // 캔버스 텍스처를 만듦
        RenderPipelineManager.beginContextRendering += OnBeginRendering;  // 매 프레임, 화면을 그리기 직전에 호출되도록 등록
    }

    void OnDisable()
    {
        RenderPipelineManager.beginContextRendering -= OnBeginRendering;  // 등록 해제
    }

    // Animator와 Look At Constraint가 이번 프레임의 움직임을 모두 끝낸 뒤 호출됨
    void OnBeginRendering(ScriptableRenderContext context, System.Collections.Generic.List<Camera> cameras)
    {
        if (canvas == null || canvas.width != width || canvas.height != height)
            CreateCanvas();                                       // Inspector에서 캔버스 크기를 바꾸면 다시 만듦

        DrawSceneOnCanvas();                                      // 이번 프레임의 장면을 캔버스에 그림
    }

    // 캔버스 텍스처를 만들고 RawImage에 연결
    void CreateCanvas()
    {
        canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);  // 가로 width, 세로 height 픽셀의 텍스처
        canvas.filterMode = FilterMode.Point;                     // 픽셀이 흐려지지 않게 그대로 보여 줌
        clearPixels = new Color[width * height];                  // 바탕색으로 채울 배열
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = backgroundColor;
        if (canvasImage != null)
        {
            canvasImage.texture = canvas;                                    // RawImage가 이 텍스처를 보여 주게 함
            canvasImage.rectTransform.sizeDelta = new Vector2(width, height); // RawImage 칸의 크기를 캔버스 픽셀 크기와 같게 맞춤 (늘어나 보이지 않게)
        }
    }


    Matrix4x4 MultiplyMatrixMatrix(Matrix4x4 A, Matrix4x4 B)
    {
        Matrix4x4 R = Matrix4x4.zero;

        for (int col = 0; col < 4; col++)
        {
            Vector4 v = B.GetColumn(col);   

            Vector4 result = A.GetColumn(0) * v.x
                           + A.GetColumn(1) * v.y
                           + A.GetColumn(2) * v.z
                           + A.GetColumn(3) * v.w;

            R.SetColumn(col, result); 
        }

        return R;
    }


    // V = R⁻¹ × T⁻¹ : 먼저 이동을 되돌리고(T⁻¹), 그다음 회전을 되돌림(R⁻¹)
    Matrix4x4 BuildViewMatrix(Transform cam)
    {
        Matrix4x4 Tinv = Matrix4x4.Translate(-cam.position);                  // T⁻¹: 이동을 되돌림
        Matrix4x4 Rinv = Matrix4x4.Rotate(Quaternion.Inverse(cam.rotation));  // R⁻¹: 회전을 되돌림
        return MultiplyMatrixMatrix(Rinv, Tinv);                                                   // V = R⁻¹ × T⁻¹
    }

    // 카메라(VirtualCamera)가 본 장면을 캔버스에 와이어프레임으로 그림 (매 프레임 호출)
    void DrawSceneOnCanvas()
    {
        ClearCanvas();                                            // 지난 프레임의 그림을 지움

        Matrix4x4 V = BuildViewMatrix(transform);                 // 이 스크립트가 붙은 VirtualCamera의 Transform으로 V를 만듦 (프레임마다 한 번)

        foreach (MeshFilter mf in targets)                        // 그릴 오브젝트마다 반복
        {
            if (mf == null || mf.sharedMesh == null) continue;    // 비어 있는 칸은 건너뜀

            Matrix4x4 VM = V * mf.transform.localToWorldMatrix;   // V × M: 오브젝트를 기준으로 잰 점을 카메라를 기준으로 잰 점으로 바꾸는 행렬 (오브젝트마다 한 번)
            Vector3[] verts = mf.sharedMesh.vertices;             // 오브젝트를 기준으로 잰 정점 배열
            int[] tris = mf.sharedMesh.triangles;                 // 정점 번호를 세 개씩 묶은 삼각형 목록

            for (int i = 0; i < tris.Length; i += 3)              // 삼각형마다 반복 (번호 세 개씩 건너뜀)
            {
                Vector3 a = VM.MultiplyPoint(verts[tris[i]]);     // 삼각형의 첫째 정점을 카메라를 기준으로 잰 좌표로
                Vector3 b = VM.MultiplyPoint(verts[tris[i + 1]]); // 둘째 정점
                Vector3 c = VM.MultiplyPoint(verts[tris[i + 2]]); // 셋째 정점

                DrawEdge(a, b);                                   // 첫째와 둘째 정점을 잇는 변
                DrawEdge(b, c);                                   // 둘째와 셋째 정점을 잇는 변
                DrawEdge(c, a);                                   // 셋째와 첫째 정점을 잇는 변
            }
        }

        canvas.Apply();                                           // 칠한 픽셀을 텍스처에 반영해 화면에 보이게 함
    }

    // 카메라를 기준으로 잰 좌표 p를 캔버스의 픽셀 위치로 바꿈. 그리지 않을 점이면 false를 돌려줌
    bool ToPixel(Vector3 p, out Vector2 pixel)
    {
        pixel = Vector2.zero;                                     // 그리지 않을 때 돌려줄 기본값

        if (p.z < near || p.z > far) return false;                // near ~ far 밖(카메라 뒤나 너무 먼 것)은 그리지 않음

        float aspect = (float)width / height;                     // 캔버스의 가로세로 비율 (가로 픽셀 ÷ 세로 픽셀)

        pixel.x = width * 0.5f                                    // 캔버스 가로 가운데에서 시작해
                + p.x / (size * aspect) * (width * 0.5f);         // x를 가로에 담을 범위로 나눈 비율(−1 ~ 1)에 가로 절반을 곱해 더함

        pixel.y = height * 0.5f                                   // 캔버스 세로 가운데에서 시작해
                + p.y / size * (height * 0.5f);                   // y를 세로에 담을 범위로 나눈 비율(−1 ~ 1)에 세로 절반을 곱해 더함

        return true;                                              // z는 위치 계산에 쓰지 않음 (정투영)
    }

    // 카메라를 기준으로 잰 두 점을 잇는 변을 캔버스에 그림
    void DrawEdge(Vector3 a, Vector3 b)
    {
        if (!ToPixel(a, out Vector2 pa)) return;                  // 한 끝이라도 그리지 않을 점이면
        if (!ToPixel(b, out Vector2 pb)) return;                  // 이 변은 그리지 않음
        DrawLine(Mathf.RoundToInt(pa.x), Mathf.RoundToInt(pa.y),  // 픽셀 위치를 정수로 반올림해
                 Mathf.RoundToInt(pb.x), Mathf.RoundToInt(pb.y)); // 두 픽셀 사이에 선을 그음
    }

    // 두 픽셀 (x0, y0), (x1, y1) 사이에 선을 그음 (브레젠험 알고리즘)
    void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;       // 가로로 갈 거리와 방향
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;      // 세로로 갈 거리와 방향 (음수로 둠)
        int err = dx + dy;                                        // 다음 픽셀을 고를 때 쓰는 오차

        while (true)
        {
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)  // 캔버스 안에 있는 픽셀만
                canvas.SetPixel(x0, y0, lineColor);               // 선 색으로 칠함

            if (x0 == x1 && y0 == y1) break;                      // 끝 픽셀에 닿으면 멈춤

            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }                // 가로로 한 칸
            if (e2 <= dx) { err += dx; y0 += sy; }                // 세로로 한 칸
        }
    }

    // 캔버스 전체를 바탕색으로 지움
    void ClearCanvas()
    {
        canvas.SetPixels(clearPixels);                            // 미리 만든 바탕색 배열로 한 번에 채움
    }

    // Scene 뷰에 VirtualCamera의 축 세 개와, 캔버스에 담기는 범위(상자)를 그림
    void OnDrawGizmos()
    {
        Vector3 o = transform.position;                           // 카메라의 기준점
        Gizmos.color = Color.red;   Gizmos.DrawLine(o, o + transform.right);    // x축: 오른쪽
        Gizmos.color = Color.green; Gizmos.DrawLine(o, o + transform.up);       // y축: 위
        Gizmos.color = Color.blue;  Gizmos.DrawLine(o, o + transform.forward);  // z축: 바라보는 방향

        float aspect = (float)width / height;                     // 캔버스의 가로세로 비율
        float hx = size * aspect, hy = size;                      // 가로 절반, 세로 절반 (유닛)
        Vector3 center = new Vector3(0, 0, (near + far) * 0.5f);  // 카메라를 기준으로 잰 상자의 가운데
        Vector3 extent = new Vector3(hx * 2, hy * 2, far - near); // 상자의 가로, 세로, 깊이

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);  // C: 카메라를 기준으로 잰 상자를 월드에 놓음
        Gizmos.DrawWireCube(center, extent);                      // near ~ far, 위아래 size, 좌우 size × aspect의 상자
        Gizmos.matrix = Matrix4x4.identity;                       // 다른 Gizmo에 영향이 없게 되돌림
    }
}