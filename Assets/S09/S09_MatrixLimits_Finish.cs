using UnityEngine;

// 3×3 행렬로 되는 것과 안 되는 것을 확인하는 스크립트
// - 스케일: 대각행렬로 성공 (다이아몬드에 바로 반영)
// - 회전: 대각행렬로는 실패 (Console에 결과 출력)
// - 이동: 어떤 3×3 행렬로도 실패 (Console에 결과 출력)
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_MatrixLimits_Finish : MonoBehaviour
{
    [Header("스케일을 대각행렬로")]
    [SerializeField] Vector3 scale = new Vector3(2f, 1f, 1f);

    [Header("회전을 대각행렬로 시도 (실패)")]
    [SerializeField] Vector3 diagonal = new Vector3(1f, 1f, 1f); // d1, d2, d3

    [Header("이동을 3×3 행렬로 시도 (실패)")]
    // Inspector는 2차원 배열을 표시하지 않으므로 세 행을 Vector3 세 개로 나눠 노출함
    [SerializeField] Vector3 row0 = new Vector3(1f, 0f, 0f);
    [SerializeField] Vector3 row1 = new Vector3(0f, 1f, 0f);
    [SerializeField] Vector3 row2 = new Vector3(0f, 0f, 1f);

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;

        Vector3[] verts = ApplyScaleMatrix(diamondMesh.BaseVertices, scale);
        diamondMesh.SetVertices(verts);
    }

    // Inspector 값을 바꿀 때마다 호출되어 Console에 바로 출력됨
    void OnValidate()
    {
        TryRotationAsDiagonalMatrix();
        TryTranslationAsMatrix();
    }

    // 3×3 행렬-벡터 곱: 결과의 각 성분 = 행렬의 한 행과 벡터의 내적
    Vector3 MultiplyMatrixVector3x3(float[,] A, Vector3 v)
    {
        float[] input = { v.x, v.y, v.z };
        float[] result = new float[3];
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 3; col++)
                result[row] += A[row, col] * input[col]; // 행과 벡터의 내적
        return new Vector3(result[0], result[1], result[2]);
    }

    // 스케일을 대각행렬로 재구현 (지난 시간 ApplyScale과 같은 결과)
    Vector3[] ApplyScaleMatrix(Vector3[] baseVertices, Vector3 s)
    {
        float[,] A = {
            { s.x, 0f,  0f  },
            { 0f,  s.y, 0f  },
            { 0f,  0f,  s.z }
        };
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = MultiplyMatrixVector3x3(A, baseVertices[i]);
        return verts;
    }

    // 회전을 대각행렬로 시도: d1, d2, d3를 어떤 값으로 바꿔도 결과는 (d1, 0, 0)
    void TryRotationAsDiagonalMatrix()
    {
        float[,] D = {
            { diagonal.x, 0f,         0f         },
            { 0f,         diagonal.y, 0f         },
            { 0f,         0f,         diagonal.z }
        };
        Vector3 v = new Vector3(1f, 0f, 0f);
        Vector3 target = new Vector3(0f, 1f, 0f);   // z축 기준 90° 회전 목표
        Vector3 result = MultiplyMatrixVector3x3(D, v);
        Debug.Log($"[회전을 대각행렬로] 결과 {result} / 목표 {target}");
    }

    // 이동을 3×3 행렬로 시도: 9개 값을 어떻게 바꿔도 원점의 결과는 (0, 0, 0)
    void TryTranslationAsMatrix()
    {
        float[,] A = {
            { row0.x, row0.y, row0.z },
            { row1.x, row1.y, row1.z },
            { row2.x, row2.y, row2.z }
        };
        Vector3 origin = Vector3.zero;
        Vector3 target = new Vector3(3f, 0f, 0f);   // t = (3, 0, 0)
        Vector3 result = MultiplyMatrixVector3x3(A, origin);
        Debug.Log($"[이동을 3×3 행렬로] 결과 {result} / 목표 {target}");
    }
}