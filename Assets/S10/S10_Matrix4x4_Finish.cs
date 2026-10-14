using UnityEngine;

// 씬: S10_Matrix4x4 · 빈 오브젝트 Matrix4x4_Test에 붙임
// Play하면 Matrix4x4의 함수로 지난 시간과 같은 계산을 하고 Console에 출력함
public class S10_Matrix4x4_Finish : MonoBehaviour
{
    void Start()
    {
        Vector3 t = new Vector3(3, 0, 0);
        Quaternion r = Quaternion.Euler(0, 0, 90);
        Vector3 s = new Vector3(2, 1, 1);

        Matrix4x4 M = Matrix4x4.TRS(t, r, s);       // 1. 한 번에: T × R × S

        Matrix4x4 T = Matrix4x4.Translate(t);
        Matrix4x4 R = Matrix4x4.Rotate(r);
        Matrix4x4 S = Matrix4x4.Scale(s);
        Matrix4x4 TRS = T * R * S;                  // 2. 따로 만들어 곱하기
        Matrix4x4 RTS = R * T * S;                  // 3. 순서를 바꾸면

        Vector3 p = M.MultiplyPoint(new Vector3(1, 0, 0));   // 4. 점에 적용

        Debug.Log("1. Matrix4x4.TRS(t, r, s)\n" + S10_LocalToWorldMatrixPrinter.Format(M));
        Debug.Log("2. T * R * S\n" + S10_LocalToWorldMatrixPrinter.Format(TRS));
        Debug.Log("3. R * T * S\n" + S10_LocalToWorldMatrixPrinter.Format(RTS));
        Debug.Log("4. M.MultiplyPoint((1, 0, 0)) = " + p);
    }
}