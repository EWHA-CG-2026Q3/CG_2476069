// DiamondMesh_Rewinded.cs
// 씬: S10의 모든 씬 / 오브젝트: 모든 다이아몬드
// S8–S12 공용 지속 오브젝트 — 다이아몬드(위/아래 사각뿔을 맞붙인 bipyramid, 6정점)
// DiamondMesh.cs의 삼각형 감는 순서(와인딩 오더)를 바로잡은 버전.
// 이 스크립트는 메시를 만들고 보관하는 역할만 함 — 이동/회전/스케일 계산은 별도 스크립트가 담당.
// 머티리얼이 비어 있거나 분홍색(셰이더 오류)이면 URP Lit 머티리얼을 자동으로 붙임.
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondMesh_Rewinded : MonoBehaviour
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

    // 바깥에서 봤을 때 시계 방향 = 앞면 (Unity 규칙)
    static readonly int[] triangles = new int[]
    {
        0,1,2, 0,2,3, 0,3,4, 0,4,1,   // 아래 사각뿔 4면
        5,2,1, 5,3,2, 5,4,3, 5,1,4,   // 위 사각뿔 4면
    };

    Mesh mesh;

    public Vector3[] BaseVertices => baseVertices;

    void OnEnable()
    {
        EnsureLitMaterial();

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

    // 머티리얼이 없거나 셰이더가 깨져 있으면(분홍색) Lit 머티리얼을 새로 만들어 붙임
    void EnsureLitMaterial()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material current = meshRenderer.sharedMaterial;

        bool needsMaterial = current == null
                          || current.shader == null
                          || !current.shader.isSupported
                          || current.shader.name == "Hidden/InternalErrorShader";
        if (!needsMaterial) return;

        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) lit = Shader.Find("Standard");   // URP가 아닌 프로젝트용 대비
        if (lit == null) return;

        meshRenderer.sharedMaterial = new Material(lit) { name = "Diamond_Lit (자동 생성)" };
    }
}