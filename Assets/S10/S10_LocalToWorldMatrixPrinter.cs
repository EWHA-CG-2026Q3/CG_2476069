using UnityEngine;

// 붙인 오브젝트의 transform.localToWorldMatrix를 Console에 출력함
// Play 없이도 동작하며, 행렬이 바뀔 때만 출력함 (Play 중에는 최대 playModeInterval초마다)
// 출력은 한 줄 = 한 행. 열은 위에서 아래로 읽음
[ExecuteAlways]
public class S10_LocalToWorldMatrixPrinter : MonoBehaviour
{
    [SerializeField] float playModeInterval = 0.5f;

    Matrix4x4 lastPrinted;
    bool hasPrinted;
    float timer;

    void OnEnable()
    {
        hasPrinted = false;
        timer = 0f;
    }

    // Animator가 이번 프레임의 값을 넣은 뒤에 읽도록 LateUpdate에서 실행
    void LateUpdate()
    {
        if (Application.isPlaying)
        {
            timer += Time.deltaTime;
            if (timer < playModeInterval) return;
            timer = 0f;
        }

        Matrix4x4 m = transform.localToWorldMatrix;
        if (hasPrinted && m == lastPrinted) return;

        lastPrinted = m;
        hasPrinted = true;
        Debug.Log($"[{name}] localToWorldMatrix\n{Format(m)}");
    }

    // 다른 스크립트에서도 같은 모양으로 출력할 수 있도록 public static
    public static string Format(Matrix4x4 m)
    {
        string s = "";
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
                s += Clean(m[row, col]).ToString("0.00").PadLeft(8);
            s += "\n";
        }
        return s;
    }

    // 지난 시간의 float[4,4] 행렬도 같은 모양으로 출력
    public static string Format(float[,] m)
    {
        string s = "";
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
                s += Clean(m[row, col]).ToString("0.00").PadLeft(8);
            s += "\n";
        }
        return s;
    }

    // cos(90°) = −0.0000000437처럼 0에 아주 가까운 값을 0으로 (−0.00 표시 방지)
    public static float Clean(float v)
    {
        return Mathf.Abs(v) < 1e-4f ? 0f : v;
    }

    public static Vector4 Clean(Vector4 v)
    {
        return new Vector4(Clean(v.x), Clean(v.y), Clean(v.z), Clean(v.w));
    }
}