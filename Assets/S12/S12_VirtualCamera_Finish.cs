using UnityEngine;
using UnityEngine.UI;

// S12: 빈 오브젝트 VirtualCamera가 본 장면을 원근투영으로 캔버스에 그림
// S11_VirtualCamera_Finish.cs에서 바뀐 곳
//   - size 대신 fieldOfView (세로 각도)
//   - BuildProjectionMatrix 추가
//   - ToPixel: size로 나누던 두 줄을 "P 곱하기와 w로 나누기"로 바꿈
//   - DrawLine: 캔버스 밖 픽셀은 칠하지 않고 건너뜀
//   - OnDrawGizmos: 직육면체 상자 대신 Frustum
public class S12_VirtualCamera_Finish : MonoBehaviour
{
    [Header("그릴 오브젝트")]
    public MeshFilter[] targets;                         // 캔버스에 그릴 오브젝트들

    [Header("카메라가 담는 범위")]
    [Range(1f, 179f)] public float fieldOfView = 90f;    // 세로 각도(도). S11의 size 자리
    public float near = 0.3f;                            // 이보다 가까운 정점은 그리지 않음
    public float far = 20f;                              // 이보다 먼 정점은 그리지 않음

    [Header("캔버스")]
    public RawImage canvasImage;                         // 캔버스를 보여 줄 RawImage
    public int width = 400;                              // 캔버스 가로 픽셀
    public int height = 300;                             // 캔버스 세로 픽셀
    public Color lineColor = Color.white;                // 선 색
    public Color clearColor = new Color(0f, 0f, 0f, 0f); // 배경색 (투명: Game 뷰 위에 겹쳐 보임)

    Texture2D canvas;                                    // 픽셀을 직접 칠하는 캔버스
    Color[] clearPixels;                                 // 지울 때 쓰는 배경색 배열
    Matrix4x4 P;                                         // 투영 행렬 (프레임마다 만듦)

    void Start()
    {
        canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);
        canvas.filterMode = FilterMode.Point;            // 픽셀이 번지지 않게
        canvas.wrapMode = TextureWrapMode.Clamp;

        clearPixels = new Color[width * height];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = clearColor;

        if (canvasImage != null) canvasImage.texture = canvas;
    }

    void LateUpdate()
    {
        DrawSceneOnCanvas();                             // 모든 오브젝트가 움직인 뒤에 그림
    }

    // V = R⁻¹ × T⁻¹ : 먼저 이동을 되돌리고(T⁻¹), 그다음 회전을 되돌림(R⁻¹) (S11과 같음)
    Matrix4x4 BuildViewMatrix(Transform cam)
    {
        Matrix4x4 Tinv = Matrix4x4.Translate(-cam.position);                    // T⁻¹
        Matrix4x4 Rinv = Matrix4x4.Rotate(Quaternion.Inverse(cam.rotation));    // R⁻¹
        return Rinv * Tinv;
    }

    // 원근 투영 행렬 P: 1행 1/(tan(fov/2) * aspect), 2행 1/tan(fov/2), 4행 [0 0 1 0]
    Matrix4x4 BuildProjectionMatrix()
    {
        float aspect = 1f;                    // 캔버스의 가로세로 비 (400 ÷ 300)
        float t = Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);   // tan(fov / 2)

        Matrix4x4 m = Matrix4x4.zero;                              // 16칸 모두 0에서 시작
        m.m00 = 1f / (t * aspect);                                 // 1행 1열
        m.m11 = 1f / t;                                            // 2행 2열
        m.m32 = 1f;                                                // 4행 3열: 4행이 [0 0 1 0], w = z
        return m;                                                  // 3행(깊이 자리)은 0 그대로
    }

    // 카메라(VirtualCamera)가 본 장면을 캔버스에 와이어프레임으로 그림 (매 프레임)
    void DrawSceneOnCanvas()
    {
        if (canvas == null) return;
        ClearCanvas();

        Matrix4x4 V = BuildViewMatrix(transform);                  // 이 스크립트가 붙은 VirtualCamera의 V
        P = BuildProjectionMatrix();                               // S12에서 늘어난 한 줄

        foreach (MeshFilter mf in targets)
        {
            if (mf == null || mf.sharedMesh == null) continue;

            Matrix4x4 VM = V * mf.transform.localToWorldMatrix;    // V × M (오브젝트마다 한 번)
            Vector3[] verts = mf.sharedMesh.vertices;
            int[] tris = mf.sharedMesh.triangles;

            for (int i = 0; i < tris.Length; i += 3)               // 삼각형마다
            {
                Vector3 a = VM.MultiplyPoint(verts[tris[i]]);      // 카메라를 기준으로 잰 좌표
                Vector3 b = VM.MultiplyPoint(verts[tris[i + 1]]);
                Vector3 c = VM.MultiplyPoint(verts[tris[i + 2]]);

                DrawEdge(a, b);
                DrawEdge(b, c);
                DrawEdge(c, a);
            }
        }

        canvas.Apply();                                            // 칠한 픽셀을 화면에 반영
    }

    void ClearCanvas()
    {
        canvas.SetPixels(clearPixels);
    }

    // 두 끝점을 픽셀로 바꾼 뒤 선을 그음. 둘 중 하나라도 그리지 않을 점이면 건너뜀
    void DrawEdge(Vector3 a, Vector3 b)
    {
        if (!ToPixel(a, out Vector2 pa)) return;
        if (!ToPixel(b, out Vector2 pb)) return;
        DrawLine(Mathf.RoundToInt(pa.x), Mathf.RoundToInt(pa.y),
                 Mathf.RoundToInt(pb.x), Mathf.RoundToInt(pb.y));
    }

    // 카메라를 기준으로 잰 좌표 p를 캔버스의 픽셀 위치로 바꿈. 그리지 않을 점이면 false
    bool ToPixel(Vector3 p, out Vector2 pixel)
    {
        pixel = Vector2.zero;
        if (p.z < near || p.z > far) return false;                   // S11과 같음. 카메라 뒤(z < 0)도 여기서 걸러짐

        Vector4 clip = P * new Vector4(p.x, p.y, p.z, 1f);           // P × (x, y, z, 1): 점이므로 네 번째 성분 1
        Vector2 ndc = new Vector2(clip.x / clip.w, clip.y / clip.w); // w(= z)로 나눔: Perspective division

        pixel.x = width * 0.5f + ndc.x * (width * 0.5f);             // 캔버스 가운데 + NDC * 가로 절반
        pixel.y = height * 0.5f + ndc.y * (height * 0.5f);           // 캔버스 가운데 + NDC * 세로 절반
        return true;
    }

    // 두 픽셀 사이에 선을 그음 (Bresenham). 캔버스 밖 픽셀은 칠하지 않고 건너뜀
    void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)   // S12에서 늘어난 확인
                canvas.SetPixel(x0, y0, lineColor);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    // Scene 뷰: VirtualCamera가 담는 범위(Frustum)와 바라보는 방향
    void OnDrawGizmos()
    {
        float aspect = (float)width / height;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.yellow;
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, far, near, aspect);  // 세로 각도, 먼 쪽, 가까운 쪽, 가로세로 비

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, Vector3.forward * 2f);               // 바라보는 방향(z축)
        Gizmos.matrix = Matrix4x4.identity;
    }
}