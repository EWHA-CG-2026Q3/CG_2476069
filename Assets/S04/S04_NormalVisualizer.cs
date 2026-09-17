using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class NormalVisualizer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) return;

        mesh.RecalculateNormals();
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
            Vector3 center = (v0 + v1 + v2) / 3f;

            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            Gizmos.DrawLine(center, center + normal * 0.5f);
        }
    }
}