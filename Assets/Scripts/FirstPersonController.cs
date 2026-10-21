using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 2.5f;
    public float gravity = -20f;

    [Header("Камера")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 85f;

    [Header("Покачивание камеры")]
    public bool headBob = true;
    public float bobFrequency = 1.6f;
    public float bobAmplitude = 0.03f;

    private CharacterController controller;
    private float xRotation;
    private float verticalVelocity;
    private float bobTimer;
    private Vector3 camStartPos;

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
    }

    void Look()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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
        Vector3 move = (transform.right * input.x + transform.forward * input.y) * moveSpeed;

        // Гравитация
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        HeadBob(input.magnitude > 0.1f && controller.isGrounded);
    }

    void HeadBob(bool isMoving)
    {
        if (!headBob) return;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency * moveSpeed;
            float offsetY = Mathf.Sin(bobTimer * Mathf.PI * 2f) * bobAmplitude;
            float offsetX = Mathf.Cos(bobTimer * Mathf.PI) * bobAmplitude * 0.5f;
            cameraTransform.localPosition = camStartPos + new Vector3(offsetX, offsetY, 0f);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, camStartPos, Time.deltaTime * 8f);
        }
    }
}