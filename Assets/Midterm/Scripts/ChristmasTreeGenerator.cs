using UnityEngine;

[ExecuteAlways]
public class ChristmasTreeGenerator : MonoBehaviour
{
    public Material leafMaterial;  // 비워두면 각 층에 직접 넣은 재질을 유지

    public int layerCount = 5;
    public float bottomRadius = 2f;
    public float bottomHeight = 1.8f;
    public float shrink = 0.75f;   // 위층일수록 작아지는 비율
    public float overlap = 0.5f;   // 층이 겹치는 정도
    public int segments = 24;

    void OnEnable()
    {
        Build();
    }

    // 인스펙터 값을 바꿀 때마다 다시 계산
    void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) Build();
        };
#endif
    }

    void Build()
    {
        float baseY = 0f;
        float radius = bottomRadius;
        float height = bottomHeight;

        for (int i = 0; i < layerCount; i++)
        {
            string layerName = "Layer_" + i;
            Transform found = transform.Find(layerName);
            GameObject layer;

            if (found != null)
            {
                layer = found.gameObject;   // 이미 있으면 재사용 (재질 유지)
            }
            else
            {
                layer = new GameObject(layerName);
                layer.transform.SetParent(transform, false);
                layer.AddComponent<MeshFilter>();
                layer.AddComponent<MeshRenderer>();
            }

            layer.transform.localPosition = new Vector3(0, baseY, 0);

            MeshFilter mf = layer.GetComponent<MeshFilter>();
            DestroyMesh(mf.sharedMesh);
            mf.sharedMesh = CreateCone(radius, height, segments);

            if (leafMaterial != null)
                layer.GetComponent<MeshRenderer>().sharedMaterial = leafMaterial;

            baseY += height * (1f - overlap);
            radius *= shrink;
            height *= shrink;
        }

        // 층 수를 줄였을 때 남는 층만 삭제
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Layer_"))
            {
                int index;
                if (int.TryParse(child.name.Substring(6), out index) && index >= layerCount)
                {
                    DestroyMesh(child.GetComponent<MeshFilter>()?.sharedMesh);
                    if (Application.isPlaying) Destroy(child.gameObject);
                    else DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    void DestroyMesh(Mesh m)
    {
        if (m == null) return;
        if (Application.isPlaying) Destroy(m);
        else DestroyImmediate(m);
    }

    Mesh CreateCone(float radius, float height, int seg)
    {
        Vector3[] vertices = new Vector3[seg * 2 + 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int apex = seg;
        int bottomStart = seg + 1;
        int bottomCenter = seg * 2 + 1;

        for (int i = 0; i < seg; i++)
        {
            float a = (float)i / seg * Mathf.PI * 2f;
            Vector3 p = new Vector3(Mathf.Cos(a) * radius, 0, Mathf.Sin(a) * radius);
            vertices[i] = p;
            vertices[bottomStart + i] = p;
            uvs[i] = new Vector2((float)i / seg, 0f);
            uvs[bottomStart + i] = new Vector2(0.5f + Mathf.Cos(a) * 0.5f, 0.5f + Mathf.Sin(a) * 0.5f);
        }
        vertices[apex] = new Vector3(0, height, 0);
        vertices[bottomCenter] = Vector3.zero;
        uvs[apex] = new Vector2(0.5f, 1f);
        uvs[bottomCenter] = new Vector2(0.5f, 0.5f);

        int[] tris = new int[seg * 6];
        int t = 0;
        for (int i = 0; i < seg; i++)
        {
            int next = (i + 1) % seg;
            tris[t++] = i;
            tris[t++] = apex;
            tris[t++] = next;
            tris[t++] = bottomCenter;
            tris[t++] = bottomStart + i;
            tris[t++] = bottomStart + next;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
}