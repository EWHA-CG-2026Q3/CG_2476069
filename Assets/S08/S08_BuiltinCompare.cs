using UnityEngine;

public class S08_BuiltinCompare : MonoBehaviour
{
    public enum DemoMode
    {
        TranslateThenRotate,
        RotateThenTranslate
    }

    [SerializeField] DemoMode demoMode = DemoMode.TranslateThenRotate;
    [SerializeField] Vector3 targetTranslation = new Vector3(3f, 0f, 0f);
    [SerializeField] float targetAngle = 90f;

    Vector3 lastTranslation;
    float lastAngle;

    void Update()
    {
        float t = Mathf.PingPong(Time.time, 1f);
        Vector3 translation = Vector3.Lerp(Vector3.zero, targetTranslation, t);
        float angle = Mathf.Lerp(0f, targetAngle, t);

        Vector3 translationDelta = translation - lastTranslation;
        float angleDelta = angle - lastAngle;

        switch (demoMode)
        {
            case DemoMode.TranslateThenRotate:
                transform.Translate(translationDelta, Space.World);
                transform.Rotate(Vector3.forward, angleDelta, Space.World);
                break;
            case DemoMode.RotateThenTranslate:
                transform.Rotate(Vector3.forward, angleDelta, Space.World);
                transform.Translate(translationDelta, Space.World);
                break;
        }

        lastTranslation = translation;
        lastAngle = angle;
    }
}