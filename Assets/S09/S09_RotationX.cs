using UnityEngine;

// 실습: RotationZMatrixRaw()를 참고해 RotationXMatrixRaw()를 완성할 것
// 확인: Transform의 Rotation x에 같은 각도를 넣은 다이아몬드와 겹치는지 비교
// (이 오브젝트의 Transform은 기본값 Position 0, Rotation 0, Scale 1을 유지)
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_RotationX : MonoBehaviour
{
    [SerializeField] float angle = 30f;   // x축 회전 각도, 도 단위

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;

        float[,] R = RotationXMatrixRaw(angle);
        Vector3[] baseVertices = diamondMesh.BaseVertices;
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = FromHomogeneous(MultiplyMatrixVectorRaw(R, ToHomogeneous(baseVertices[i])));
        diamondMesh.SetVertices(verts);
    }

    // 참고: z축 회전 행렬 (e₃는 그대로, e₁과 e₂가 돎)
    float[,] RotationZMatrixRaw(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new float[,] {
            { c,  -s,  0f, 0f },
            { s,   c,  0f, 0f },
            { 0f,  0f, 1f, 0f },
            { 0f,  0f, 0f, 1f }
        };
    }

    // x축 회전 행렬 (e₁은 그대로, e₂와 e₃가 돎)
    float[,] RotationXMatrixRaw(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        // TODO: x축 회전 행렬을 float[4,4]로 반환
        //       (4열과 4행은 z축 회전과 같음)
        //       완성하면 아래의 임시 반환(단위행렬)을 지울 것

        return new float[,] {   // 임시: 아무 변환도 하지 않는 단위행렬

            {1f, 0f, 0f, 0f},
            {0f, c, -s, 0f},
            {0f, s, c, 0f},
            {0f, 0f, 0f, 1f}
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