using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float timer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private int frames;

    void Update()
    {
        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= 1f)
        {
            Debug.Log("FPS: " + frames);

            frames = 0;
            timer = 0f;
        }
    }
}
