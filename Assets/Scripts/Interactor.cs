using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Взгляд")]
    public Transform cam;
    public float distance = 2.5f;
    private string interactionLayerName = "Pickable";

    [Header("Door Sounds")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    int interactionLayer;

    void Awake()
    {
        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        interactionLayer = LayerMask.NameToLayer(interactionLayerName);
        if (interactionLayer < 0)
            Debug.LogError($"Слой \"{interactionLayerName}\" не создан (Inspector → Layer → Add Layer).");
    }

    void Update()
    {
        if (cam == null || interactionLayer < 0) return;

        bool looking = TryGetTarget(out RaycastHit hit);
        if (!looking && Physics.Raycast(cam.position, cam.forward, out var h, distance))
            Debug.Log($"{h.collider.name} | layer={LayerMask.LayerToName(h.collider.gameObject.layer)}");
        if (GameLogic.Instance != null)
            GameLogic.Instance.ShowClue(looking);

        if (looking && Input.GetKeyDown(KeyCode.E))
            UseDoor(hit);
    }

    // Луч бьёт во всё подряд, поэтому стена перед объектом закроет подсказку
    bool TryGetTarget(out RaycastHit hit)
    {
        if (!Physics.Raycast(cam.position, cam.forward, out hit, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            return false;

        // Слой может стоять на родителе, а коллайдер на дочернем объекте
        for (Transform t = hit.collider.transform; t != null; t = t.parent)
            if (t.gameObject.layer == interactionLayer) return true;

        return false;
    }

    void UseDoor(RaycastHit hit)
    {
        Animator doorAnimator = hit.collider.GetComponentInParent<Animator>();
        if (doorAnimator == null) return;

        bool newState = !doorAnimator.GetBool("isOpen");
        doorAnimator.SetBool("isOpen", newState);

        AudioClip sound = newState ? openSound : closeSound;
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);
    }
}