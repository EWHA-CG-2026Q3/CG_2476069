using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S03_CustomPolygonMesh_Pentagon : MonoBehaviour
{
    void Start()
    {
        // TODO 1: 원하는 다각형의 정점 좌표를 채우세요 (최소 4개)
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f), 
            new Vector3(2f, 0f, 0f), 
            new Vector3(2f, 1f, 0f), 
            new Vector3(1f, 2f, 0f), 
            new Vector3(0f, 1f, 0f), 
        };

        // TODO 2: 정점 3개씩 묶어 삼각형들을 구성하세요
        int[] triangles = new int[4]
        {
              // 예: 0, 1, 2,
              0, 1, 2,
              0, 2, 3,

        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}