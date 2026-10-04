// DiamondMesh.cs
// S8-S12 공용 지속 오브젝트 — 다이아몬드(위/아래 사각뿔을 맞붙인 bipyramid, 6정점)
// S04 출석인정과제의 정답이기도 함.
// 이 스크립트는 메시를 만들고 보관하는 역할만 함 — 이동/회전/스케일 계산은 별도 스크립트가 담당.
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondMesh : MonoBehaviour
{
    [Header("다이아몬드 원본 정점 (0=아래 꼭짓점, 1~4=중간 사각형, 5=위 꼭짓점)")]
    [SerializeField]
    Vector3[] baseVertices = new Vector3[]
    {
        new Vector3(0.5f, 0f,   0.5f), // 0 — 아래 꼭짓점
        new Vector3(0f,   0.5f, 0f),   // 1 — 중간 사각형
        new Vector3(1f,   0.5f, 0f),   // 2
        new Vector3(1f,   0.5f, 1f),   // 3
        new Vector3(0f,   0.5f, 1f),   // 4
        new Vector3(0.5f, 1f,   0.5f), // 5 — 위 꼭짓점
    };

    static readonly int[] triangles = new int[]
    {
        0,2,1, 0,3,2, 0,4,3, 0,1,4,   // 아래 사각뿔 4면
        5,1,2, 5,2,3, 5,3,4, 5,4,1,   // 위 사각뿔 4면
    };

    Mesh mesh;

    public Vector3[] BaseVertices => baseVertices;

    void OnEnable()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        SetVertices(baseVertices);
    }

    public void SetVertices(Vector3[] verts)
    {
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}