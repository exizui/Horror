using UnityEngine;

public class CandleController : MonoBehaviour
{
    [Header("Огонь (если пусто, найдётся в дочерних)")]
    [SerializeField] Light flameLight;
    [SerializeField] Renderer flameRenderer;

    [Header("Горение")]
    [SerializeField] float burnDuration = 60f;
    [SerializeField] float dyingTime = 10f;

    [Header("Звук (необязательно)")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip extinguishClip;
    [SerializeField] AudioClip igniteClip;
    public bool IsLit { get; private set; }

    Transform flameTransform;
    Vector3 baseScale;
    float baseIntensity;
    float remaining;
    float draft = 1f;

    void Awake()
    {
        if (flameLight == null) flameLight = GetComponentInChildren<Light>();
        if (flameRenderer == null) flameRenderer = flameLight.GetComponent<Renderer>();

        flameTransform = flameLight.transform;
        baseScale = flameTransform.localScale;
        baseIntensity = flameLight.intensity;

        remaining = burnDuration;
        IsLit = true;
    }

    void Update()
    {
        if (!IsLit) return;

        remaining -= Time.deltaTime * draft;
        if (remaining <= 0f) { Extinguish(); return; }

        bool dying = remaining < dyingTime;
        float fade = dying ? remaining / dyingTime : 1f;
        float noise = Mathf.PerlinNoise(Time.time * (dying ? 14f : 4f), 0f);
        float flicker = dying ? 0.5f + noise : 0.9f + noise * 0.2f;

        flameLight.intensity = baseIntensity * fade * flicker;
        flameTransform.localScale = baseScale * Mathf.Lerp(0.3f, 1f, fade);
    }

    // Можно вызывать из скримеров и триггеров
    public void Extinguish()
    {
        if (!IsLit) return;

        IsLit = false;
        remaining = 0f;
        flameLight.enabled = false;
        if (flameRenderer != null) flameRenderer.enabled = false;

        if (audioSource != null && extinguishClip != null)
            audioSource.PlayOneShot(extinguishClip);
    }

    // Сквозняк: 2 = сгорает вдвое быстрее, 1 = норма
    public void SetDraft(float multiplier) => draft = Mathf.Max(1f, multiplier);

    public void Ignite()
    {
        remaining = burnDuration;
        IsLit = true;

        flameLight.enabled = true;
        flameLight.intensity = baseIntensity;
        flameTransform.localScale = baseScale;
        if (flameRenderer != null) flameRenderer.enabled = true;

        if (audioSource != null && igniteClip != null)
            audioSource.PlayOneShot(igniteClip);
    }
}