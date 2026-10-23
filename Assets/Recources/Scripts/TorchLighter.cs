using UnityEngine;

public class TorchLighter : MonoBehaviour
{
    [SerializeField] CandleController candle;
    [SerializeField] Transform cam;
    [SerializeField] float distance = 2.5f;
    [SerializeField] string torchTag = "Torch";
    [SerializeField] KeyCode key = KeyCode.E;
    [SerializeField] GameObject prompt; // подсказка, необязательно

    void Awake()
    {
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        if (candle == null) candle = GetComponentInChildren<CandleController>(true);
        if (candle == null) candle = FindAnyObjectByType<CandleController>();

        //if (prompt == null)
        //{
        //    Canvas canvas = FindAnyObjectByType<Canvas>();

        //    if (canvas != null)
        //    {
        //        Transform found = canvas.transform.Find("pressE");

        //        if (found != null)
        //            prompt = found.gameObject;
        //    }
        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && LookingAtTorch())
            candle.Ignite();
    }

    bool LookingAtTorch()
    {
        if (cam == null || candle == null) return false;

        if (!Physics.Raycast(cam.position, cam.forward, out var hit, distance,
                ~0, QueryTriggerInteraction.Collide))
            return false;

        // Тег может стоять на родителе, поэтому поднимаемся вверх по иерархии
        for (Transform t = hit.collider.transform; t != null; t = t.parent)
            if (t.CompareTag(torchTag)) return true;

        return false;
    }
}
