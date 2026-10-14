using UnityEngine;

// 씬: S10_BouncingBallAnimation · Diamond_Mesh에 붙임 (DiamondMesh_Rewinded와 함께)
// Hierarchy 대신 chain 배열의 순서대로 행렬을 곱해 다이아몬드 꼭짓점을 직접 계산함
// chain의 오브젝트와 Diamond_Mesh는 모두 맨 위 단계에 두고, Diamond_Mesh의 Transform은 기본값으로 둠
// Scene 뷰에는 곱셈 단계마다 그때까지 합친 행렬의 축 세 개(1–3열)와 기준점(4열)을 그림
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh_Rewinded))]
public class S10_DiamondChain_Finish : MonoBehaviour
{
    // 부모부터 차례로: DiamondStage, Diamond_Orbit, Diamond_Offset, Diamond_Ball
    [SerializeField] Transform[] chain;
    [SerializeField] float axisLength = 0.15f;   // 화면에 그리는 축의 길이

    // Animator가 이번 프레임의 값을 넣은 뒤에 실행
    void LateUpdate()
    {
        if (chain == null) return;

        // 1. 부모부터 차례로 오른쪽에 곱해 행렬 하나로 합침
        Matrix4x4 M = Matrix4x4.identity;
        foreach (Transform node in chain)
        {
            if (node == null) continue;
            M = M * Matrix4x4.TRS(node.localPosition, node.localRotation, node.localScale);
        }

        // 2. 꼭짓점마다 한 번 곱함
        DiamondMesh_Rewinded diamond = GetComponent<DiamondMesh_Rewinded>();
        Vector3[] baseVertices = diamond.BaseVertices;
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = M.MultiplyPoint(baseVertices[i]);
        diamond.SetVertices(verts);
    }

    // 곱셈 단계마다: 4열 = 기준점, 1–3열 = 축 세 개
    void OnDrawGizmos()
    {
        if (chain == null) return;

        Matrix4x4 M = Matrix4x4.identity;
        Vector3 prevOrigin = Vector3.zero;
        bool first = true;

        foreach (Transform node in chain)
        {
            if (node == null) continue;
            M = M * Matrix4x4.TRS(node.localPosition, node.localRotation, node.localScale);

            Vector3 origin = M.GetColumn(3);                     // 4열: 기준점
            Vector3 e1 = ((Vector3)M.GetColumn(0)).normalized;   // 1열: e₁의 방향
            Vector3 e2 = ((Vector3)M.GetColumn(1)).normalized;   // 2열: e₂의 방향
            Vector3 e3 = ((Vector3)M.GetColumn(2)).normalized;   // 3열: e₃의 방향

            Gizmos.color = Color.red;   Gizmos.DrawLine(origin, origin + e1 * axisLength);
            Gizmos.color = Color.green; Gizmos.DrawLine(origin, origin + e2 * axisLength);
            Gizmos.color = Color.blue;  Gizmos.DrawLine(origin, origin + e3 * axisLength);

            // 부모의 기준점에서 이 단계의 기준점까지 잇는 선
            Gizmos.color = Color.yellow;
            if (!first) Gizmos.DrawLine(prevOrigin, origin);
            Gizmos.DrawWireSphere(origin, axisLength * 0.15f);

            prevOrigin = origin;
            first = false;
        }
    }
}