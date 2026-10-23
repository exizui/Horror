using UnityEngine;
using UnityEngine.InputSystem;


public class CameraInertia : MonoBehaviour
{
    [Header("Включение")]
    public bool inertiaEnabled = true;

    [Header("Инерция")]
    [Tooltip("Время догоняния. Больше = тяжелее и плавнее")]
    public float smoothTime = 0.08f;

    [Tooltip("Максимальное отставание камеры (градусы)")]
    public float maxLag = 15f;

    [Tooltip("Скорость плавного вкл/выкл")]
    public float fadeSpeed = 6f;

    Transform body;          // Player (родитель)
    float smoothYaw, yawVel;
    float smoothPitch, pitchVel;
    float weight;
    bool initialized;

    void OnEnable()
    {
        initialized = false;
    }

    void LateUpdate()
    {
        if (body == null) body = transform.parent;
        if (body == null) return;

        // Реальные углы, выставленные FirstPersonController
        float targetYaw = body.eulerAngles.y;
        float targetPitch = NormalizeAngle(transform.localEulerAngles.x);

        if (!initialized)
        {
            smoothYaw = targetYaw;
            smoothPitch = targetPitch;
            weight = inertiaEnabled ? 1f : 0f;
            initialized = true;
            return;
        }

        weight = Mathf.MoveTowards(weight, inertiaEnabled ? 1f : 0f, fadeSpeed * Time.deltaTime);

        // Сглаженные углы догоняют реальные
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, targetYaw, ref yawVel, smoothTime);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, targetPitch, ref pitchVel, smoothTime);

        // Отставание = разница между сглаженным и реальным
        float yawLag = Mathf.Clamp(Mathf.DeltaAngle(targetYaw, smoothYaw), -maxLag, maxLag);
        float pitchLag = Mathf.Clamp(Mathf.DeltaAngle(targetPitch, smoothPitch), -maxLag, maxLag);

        // Добавляем только смещение, базовый поворот остаётся от контроллера
        transform.localRotation = Quaternion.Euler(
            targetPitch + pitchLag * weight,
            yawLag * weight,
            0f);
    }

    static float NormalizeAngle(float a)
    {
        return a > 180f ? a - 360f : a;
    }

    public void SetInertia(bool value)
    {
        inertiaEnabled = value;
    }
}