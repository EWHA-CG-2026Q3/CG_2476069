
using UnityEngine;

public class GiftOrbitRotation : MonoBehaviour
{
    // 회전 속도
    public float rotationSpeed = 15f;

    void Update()
    {
        // Y축을 기준으로 부모 오브젝트 회전
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f,
            Space.Self
        );
    }
}
