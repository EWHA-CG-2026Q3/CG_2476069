
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChristmasStar : MonoBehaviour
{
    public float outerRadius = 0.35f;
    public float innerRadius = 0.15f;
    public float depth = 0.12f;

    void Start()
    {
        CreateStar();
    }

    void CreateStar()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[22];

        // ¾Õ¸é°ú µÞ¸éÀÇ Áß½ÉÁ¡
        vertices[0] = new Vector3(0, 0, depth / 2);
        vertices[11] = new Vector3(0, 0, -depth / 2);

        // º°ÀÇ ¾Õ¸é°ú µÞ¸é ²ÀÁþÁ¡
        for (int i = 0; i < 10; i++)
        {
            float angle = Mathf.PI / 2f + i * Mathf.PI / 5f;

            float radius = (i % 2 == 0)
                ? outerRadius
                : innerRadius;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            vertices[i + 1] = new Vector3(x, y, depth / 2);
            vertices[i + 12] = new Vector3(x, y, -depth / 2);
        }

        int[] triangles = new int[120];
        int t = 0;

        // ¾Õ¸é
        for (int i = 0; i < 10; i++)
        {
            int next = (i + 1) % 10;

            triangles[t++] = 0;
            triangles[t++] = i + 1;
            triangles[t++] = next + 1;
        }

        // µÞ¸é
        for (int i = 0; i < 10; i++)
        {
            int next = (i + 1) % 10;

            triangles[t++] = 11;
            triangles[t++] = next + 12;
            triangles[t++] = i + 12;
        }

        // ¿·¸é
        for (int i = 0; i < 10; i++)
        {
            int next = (i + 1) % 10;

            int frontA = i + 1;
            int frontB = next + 1;
            int backA = i + 12;
            int backB = next + 12;

            triangles[t++] = frontA;
            triangles[t++] = backA;
            triangles[t++] = frontB;

            triangles[t++] = frontB;
            triangles[t++] = backA;
            triangles[t++] = backB;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
