using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Главный переключатель")]
    [Tooltip("Разрешить инерцию (движение, взгляд, крен, приземление)")]
    public bool inertiaEnabled = true;

    [Header("Движение")]
    public float moveSpeed = 2.5f;
    public float gravity = -20f;

    [Header("Инерция движения")]
    public float acceleration = 12f;
    public float deceleration = 9f;
    [Range(0f, 1f)] public float airControl = 0.3f;

    [Header("Камера")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 85f;

    [Header("Инерция взгляда")]
    public float lookSmooth = 22f;

    [Header("Крен камеры при стрейфе")]
    public float tiltAngle = 1.5f;
    public float tiltSmooth = 7f;

    [Header("Приземление")]
    public float landDipAmount = 0.12f;
    public float landDipMinSpeed = 6f;
    public float landRecoverSpeed = 6f;

    [Header("Покачивание камеры")]
    public bool headBob = true;
    public float bobFrequency = 1.6f;
    public float bobAmplitude = 0.03f;

    private CharacterController controller;
    private float xRotation;
    private float verticalVelocity;
    private float bobTimer;
    private Vector3 camStartPos;

    private Vector3 horizontalVelocity;
    private Vector2 smoothedDelta;
    private float currentRoll;
    private float landOffset;
    private bool wasGrounded;
    private float lastFallSpeed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        camStartPos = cameraTransform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
        UpdateCameraPosition();
    }

    void Look()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        if (inertiaEnabled)
        {
            // Лёгкая тяжесть взгляда
            float k = 1f - Mathf.Exp(-lookSmooth * Time.deltaTime);
            smoothedDelta = Vector2.Lerp(smoothedDelta, delta, k);
            delta = smoothedDelta;
        }
        else
        {
            smoothedDelta = Vector2.zero;
        }

        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Крен при стрейфе (если инерция выключена, плавно возвращается в 0)
        float rollTarget = 0f;
        if (inertiaEnabled)
        {
            float sideSpeed = Vector3.Dot(horizontalVelocity, transform.right);
            rollTarget = -(sideSpeed / Mathf.Max(0.01f, moveSpeed)) * tiltAngle;
        }
        currentRoll = Mathf.Lerp(currentRoll, rollTarget, 1f - Mathf.Exp(-tiltSmooth * Time.deltaTime));

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, currentRoll);
        transform.Rotate(Vector3.up * delta.x);
    }

    void Move()
    {
        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;

        if (kb != null)
        {
            if (kb.wKey.isPressed) input.y += 1;
            if (kb.sKey.isPressed) input.y -= 1;
            if (kb.dKey.isPressed) input.x += 1;
            if (kb.aKey.isPressed) input.x -= 1;
        }

        input = Vector2.ClampMagnitude(input, 1f);
        Vector3 targetVelocity = (transform.right * input.x + transform.forward * input.y) * moveSpeed;

        bool grounded = controller.isGrounded;

        if (inertiaEnabled)
        {
            bool hasInput = input.sqrMagnitude > 0.01f;
            float rate = hasInput ? acceleration : deceleration;
            if (!grounded) rate *= airControl;

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, rate * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = targetVelocity;
        }

        // Гравитация
        if (grounded && verticalVelocity < 0f)
        {
            // Просадка при приземлении только с инерцией
            if (inertiaEnabled && !wasGrounded && lastFallSpeed > landDipMinSpeed)
            {
                float t = Mathf.InverseLerp(landDipMinSpeed, landDipMinSpeed * 3f, lastFallSpeed);
                landOffset = -landDipAmount * Mathf.Lerp(0.4f, 1f, t);
            }
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        if (!grounded) lastFallSpeed = -verticalVelocity;
        wasGrounded = grounded;

        Vector3 move = horizontalVelocity;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    void UpdateCameraPosition()
    {
        Vector3 offset = Vector3.zero;

        float speed = new Vector3(horizontalVelocity.x, 0f, horizontalVelocity.z).magnitude;
        bool isMoving = speed > 0.1f && controller.isGrounded;

        if (headBob)
        {
            if (isMoving)
            {
                bobTimer += Time.deltaTime * bobFrequency * speed;
                float amp = Mathf.Clamp01(speed / Mathf.Max(0.01f, moveSpeed));
                float offsetY = Mathf.Sin(bobTimer * Mathf.PI * 2f) * bobAmplitude * amp;
                float offsetX = Mathf.Cos(bobTimer * Mathf.PI) * bobAmplitude * 0.5f * amp;
                offset += new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                bobTimer = 0f;
            }
        }

        // Просадка возвращается плавно (при выключенной инерции просто затухает)
        landOffset = Mathf.Lerp(landOffset, 0f, Time.deltaTime * landRecoverSpeed);
        offset.y += landOffset;

        Vector3 targetPos = camStartPos + offset;
        float follow = isMoving || Mathf.Abs(landOffset) > 0.001f ? 20f : 8f;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * follow);
    }

    // Для вызова из других скриптов
    public void SetInertia(bool value)
    {
        inertiaEnabled = value;
    }
}