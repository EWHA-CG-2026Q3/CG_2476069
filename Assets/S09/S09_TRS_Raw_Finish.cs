using UnityEngine;

// 동차좌표와 4×4 행렬로 스케일 → 회전 → 이동을 적용하는 스크립트
// Unity의 Matrix4x4 타입 없이 float[4,4] 배열만으로 계산함
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_TRS_Raw_Finish : MonoBehaviour
{
    [SerializeField] Vector3 t = new Vector3(3f, 0f, 0f);   // 이동 (Position)
    [SerializeField] float angle = 90f;                      // z축 회전 각도, 도 단위 (Rotation z)
    [SerializeField] Vector3 s = new Vector3(2f, 1f, 1f);    // 스케일 (Scale)

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;

        Vector3[] verts = ApplyTRS_Raw(diamondMesh.BaseVertices, t, angle, s);
        diamondMesh.SetVertices(verts);
    }

    // "S, R, T를 차례로 적용 (실행 시나리오)"의 값을 Console에서 확인
    void OnValidate()
    {
        float[,] T = TranslationMatrixRaw(t);
        float[,] R = RotationZMatrixRaw(angle);
        float[,] S = ScaleMatrixRaw(s);

        Vector4 h = ToHomogeneous(new Vector3(1f, 0f, 0f));
        Debug.Log($"v = {h}");
        h = MultiplyMatrixVectorRaw(S, h); Debug.Log($"S 적용 → {h}");
        h = MultiplyMatrixVectorRaw(R, h); Debug.Log($"R 적용 → {h}");
        h = MultiplyMatrixVectorRaw(T, h); Debug.Log($"T 적용 → {h}");
        Debug.Log($"네 번째 성분을 떼면 → {FromHomogeneous(h)}");
    }

    // ---------- 행렬 빌더 ----------

    // 1~3열: e₁, e₂, e₃ 그대로 / 4열: 원점이 t로 이동
    float[,] TranslationMatrixRaw(Vector3 t)
    {
        return new float[,] {
            { 1f, 0f, 0f, t.x },
            { 0f, 1f, 0f, t.y },
            { 0f, 0f, 1f, t.z },
            { 0f, 0f, 0f, 1f  }
        };
    }

    // 1~3열: e₁, e₂, e₃가 각각 sx, sy, sz배 / 4열: 원점 그대로
    float[,] ScaleMatrixRaw(Vector3 s)
    {
        return new float[,] {
            { s.x, 0f,  0f,  0f },
            { 0f,  s.y, 0f,  0f },
            { 0f,  0f,  s.z, 0f },
            { 0f,  0f,  0f,  1f }
        };
    }

    // 1열: e₁ → (c, s, 0) / 2열: e₂ → (−s, c, 0) / 3열: e₃ 그대로 / 4열: 원점 그대로
    float[,] RotationZMatrixRaw(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad; // Mathf.Cos, Sin은 라디안을 받음
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new float[,] {
            { c,  -s,  0f, 0f },
            { s,   c,  0f, 0f },
            { 0f,  0f, 1f, 0f },
            { 0f,  0f, 0f, 1f }
        };
    }

    // ---------- 동차좌표 ----------

    // 정점에 네 번째 성분 1을 붙임
    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    // 계산이 끝난 뒤 네 번째 성분을 떼어냄
    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }

    // 4×4 행렬-벡터 곱: MultiplyMatrixVector3x3과 구조가 같고 반복 횟수만 4
    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input = { v.x, v.y, v.z, v.w };
        float[] result = new float[4];
        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                result[row] += M[row, col] * input[col]; // 행과 벡터의 내적
        return new Vector4(result[0], result[1], result[2], result[3]);
    }

    // ---------- 적용 ----------

    // 모든 정점에 스케일 → 회전 → 이동을 차례로 적용
    Vector3[] ApplyTRS_Raw(Vector3[] baseVertices, Vector3 t, float angleDegrees, Vector3 s)
    {
        float[,] T = TranslationMatrixRaw(t);
        float[,] R = RotationZMatrixRaw(angleDegrees);
        float[,] S = ScaleMatrixRaw(s);

        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector4 h = ToHomogeneous(baseVertices[i]);
            h = MultiplyMatrixVectorRaw(S, h);   // 1. 스케일
            h = MultiplyMatrixVectorRaw(R, h);   // 2. 회전
            h = MultiplyMatrixVectorRaw(T, h);   // 3. 이동
            verts[i] = FromHomogeneous(h);
        }
        return verts;
    }
}