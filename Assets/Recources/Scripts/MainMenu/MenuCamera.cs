using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Header("Позиция камеры")]
    public float positionRange = 0.15f;
    public float positionSpeed = 0.5f;

    [Header("Поворот камеры")]
    public float rotationRange = 2f;
    public float rotationSpeed = 0.3f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float seedX;
    private float seedY;
    private float seedZ;

    void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
        seedZ = Random.Range(0f, 100f);
    }

    void Update()
    {
        float time = Time.time;

        // Плавное движение позиции
        float x = Mathf.PerlinNoise(seedX, time * positionSpeed) - 0.5f;
        float y = Mathf.PerlinNoise(seedY, time * positionSpeed) - 0.5f;
        float z = Mathf.PerlinNoise(seedZ, time * positionSpeed) - 0.5f;

        Vector3 offset = new Vector3(x, y, z) * positionRange;

        transform.localPosition = startPosition + offset;

        // Плавный поворот камеры
        float rotX = (Mathf.PerlinNoise(seedX + 10f, time * rotationSpeed) - 0.5f) * rotationRange;
        float rotY = (Mathf.PerlinNoise(seedY + 10f, time * rotationSpeed) - 0.5f) * rotationRange;
        float rotZ = (Mathf.PerlinNoise(seedZ + 10f, time * rotationSpeed) - 0.5f) * rotationRange;

        transform.localRotation = startRotation * Quaternion.Euler(rotX, rotY, rotZ);
    }
}
