
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;

        public void Play(AudioSource src)
        {
            if (src != null && clip != null)
                src.PlayOneShot(clip, volume);
        }
    }

    [Header("Звуки (у каждого своя громкость)")]
    public Sound doorSlam;
    public Sound firstEnb;

    [SerializeField] float delay = 1.5f;
    bool used;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used || !other.CompareTag("Player"))
            return;

        used = true;

        GameObject player = other.transform.root.gameObject;
        DontDestroyOnLoad(player);

        StartCoroutine(LoadAfterDelay(player));
    }

    IEnumerator LoadAfterDelay(GameObject player)
    {
        var src = player.GetComponentInChildren<AudioSource>();

        yield return new WaitForSeconds(delay);

        doorSlam.Play(src);
        firstEnb.Play(src);

        SceneManager.sceneLoaded += OnSceneLoaded;
        yield return SceneManager.LoadSceneAsync(2);    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (player.GetComponent<TorchLighter>() == null)
            player.AddComponent<TorchLighter>();
    }
}