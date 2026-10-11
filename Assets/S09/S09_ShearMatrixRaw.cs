
using UnityEngine;

// S09 과제: Shear 변환
// 학번 마지막 숫자 = 9
// k = (9 + 1) / 5 = 2

[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_ShearMatrixRaw : MonoBehaviour
{
    [SerializeField] float shearK = 2f;   // Shear 계수: 2 또는 -2

    DiamondMesh diamondMesh;
    float lastLoggedK = float.NaN;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;

        float[,] S = ShearMatrixRaw(shearK);
        Vector3[] baseVertices = diamondMesh.BaseVertices;
        Vector3[] verts = new Vector3[baseVertices.Length];

        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = FromHomogeneous(
                MultiplyMatrixVectorRaw(S, ToHomogeneous(baseVertices[i]))
            );

        diamondMesh.SetVertices(verts);

        // 과제에서 요구한 꼭짓점 변환 결과 출력
        if (float.IsNaN(lastLoggedK) ||
            Mathf.Abs(lastLoggedK - shearK) > 0.0001f)
        {
            Vector3 topVertex = new Vector3(0.5f, 1f, 0.5f);

            Vector3 transformedTop = FromHomogeneous(
                MultiplyMatrixVectorRaw(S, ToHomogeneous(topVertex))
            );

            Debug.Log(
                $"Shear k = {shearK} | " +
                $"Top vertex {topVertex} -> {transformedTop}"
            );

            lastLoggedK = shearK;
        }
    }

    // Shear 행렬: (x, y, z) -> (x + k*y, y, z)
    float[,] ShearMatrixRaw(float k)
    {
        return new float[,] {
            { 1f,  k,  0f, 0f },
            { 0f,  1f,  0f, 0f },
            { 0f,  0f,  1f, 0f },
            { 0f,  0f,  0f, 1f }
        };
    }

    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }

    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input = { v.x, v.y, v.z, v.w };
        float[] result = new float[4];

        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                result[row] += M[row, col] * input[col];

        return new Vector4(result[0], result[1], result[2], result[3]);
    }
}
