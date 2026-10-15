using UnityEngine;

// PyramidMesh — 여러 회차에서 함께 쓰는 사각뿔 메시
// 바닥 정사각형 꼭짓점 4개 + 꼭대기 1개 = 5개, 삼각형 6개 (바닥 2 + 옆면 4)
// 좌표는 S03/S04와 같은 0/1 좌표계
// 삼각형은 바깥에서 볼 때 시계 방향(Unity의 앞면)이 되도록 순서를 맞춤
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PyramidMesh : MonoBehaviour
{
    Vector3[] baseVertices =
    {
        new Vector3(0f,   0f, 0f),     // 0  바닥 왼쪽 앞
        new Vector3(1f,   0f, 0f),     // 1  바닥 오른쪽 앞
        new Vector3(1f,   0f, 1f),     // 2  바닥 오른쪽 뒤
        new Vector3(0f,   0f, 1f),     // 3  바닥 왼쪽 뒤
        new Vector3(0.5f, 1f, 0.5f),   // 4  꼭대기
    };

    int[] triangles =
    {
        0, 1, 2,   0, 2, 3,   // 바닥 (아래를 향함)
        0, 4, 1,              // 앞면 (z = 0 쪽)
        1, 4, 2,              // 오른쪽면 (x = 1 쪽)
        2, 4, 3,              // 뒷면 (z = 1 쪽)
        3, 4, 0,              // 왼쪽면 (x = 0 쪽)
    };

    Mesh mesh;

    // 원래 꼭짓점 (변환 스크립트가 읽기만 함)
    public Vector3[] BaseVertices => baseVertices;

    void OnEnable()
    {
        EnsureLitMaterial();
        BuildMesh();
    }

    void BuildMesh()
    {
        mesh = new Mesh();
        mesh.name = "Pyramid";
        mesh.vertices = baseVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // 변환 스크립트가 계산한 꼭짓점을 메시에 씀 (baseVertices는 바뀌지 않음)
    public void SetVertices(Vector3[] verts)
    {
        if (mesh == null) BuildMesh();
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

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

        meshRenderer.sharedMaterial = new Material(lit) { name = "Pyramid_Lit (자동 생성)" };
    }
}