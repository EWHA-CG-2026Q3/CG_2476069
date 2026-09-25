using UnityEngine;

public class S06_ImmediateModeTriangle_Finish : MonoBehaviour
{
    [SerializeField] private Material glMaterial;
    [SerializeField] private Vector3 vertexA = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 vertexB = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 vertexC = new Vector3(0.5f, 1f, 0f);
    [SerializeField] private Color triangleColor = new Color(0.6f, 1f, 0.2f, 1f);

    void OnRenderObject()
    {
        // 이 GameObject가 활성화되어 있으면, 매 카메라 렌더링마다 Unity가 자동으로 호출함
        // (Renderer 컴포넌트 없이도 동작 — 빈 GameObject에 이 스크립트만 붙여도 됨)

        glMaterial.SetPass(0);
        // lineMaterial이 사용할 셰이더를 GPU에 적용(활성화)
        // 이후 그리는 모든 정점은 이 셰이더로 처리됨

        GL.Begin(GL.TRIANGLES);
        // "지금부터 정점 3개씩 묶어서 삼각형으로 그리겠다"고 GPU에 선언
        // (GL.LINES, GL.QUADS 등 다른 모드도 있음)

        GL.Color(triangleColor);
        // 지금부터 그릴 정점들의 색을 지정
        // (이 색은 lineMaterial의 셰이더가 실제로 읽어서 써야 반영됨 — Lit 셰이더는 무시함)

        GL.Vertex3(vertexA.x, vertexA.y, vertexA.z);
        GL.Vertex3(vertexB.x, vertexB.y, vertexB.z);
        GL.Vertex3(vertexC.x, vertexC.y, vertexC.z);
        // 정점 세 개를 순서대로 GPU에 넘김 — 이 세 점을 어떻게 픽셀로 채울지는
        // 우리가 계산하지 않고 GPU가 알아서 처리(래스터화)
        // 좌표는 LoadOrtho()를 쓰지 않았으므로 씬의 3D 월드 좌표로 해석됨

        GL.End();
        // "정점 입력 끝" — 여기까지 받은 정점들로 실제 삼각형을 그림
    }
}
