using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource soundSource; // можно назначить вручную
    //public AudioClip door_slam;
    //public AudioClip first_emb;

    void Awake()
    {
        // Дубликат из новой сцены удаляется, остаётся оригинал с назначенными клипами
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AudioSource SoundSource
    {
        get
        {
            if (soundSource != null) return soundSource;

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return null;

            foreach (var src in player.GetComponentsInChildren<AudioSource>(true))
            {
                if (src.gameObject.name == "SoundSource")
                {
                    soundSource = src;
                    break;
                }
            }

            // Запасной вариант: любой AudioSource игрока
            if (soundSource == null)
                soundSource = player.GetComponentInChildren<AudioSource>(true);

            return soundSource;
        }
    }

    public void Play(AudioSource src, AudioClip clip)
    {
        if (src == null)
        {
            Debug.LogWarning("SoundManager: у Player нет AudioSource (ищется дочерний объект \"SoundSource\").");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: клип не назначен в Inspector.");
            return;
        }

        src.PlayOneShot(clip);
    }

    //public void DoorSlam() => Play(SoundSource, door_slam);

    //public void First_Embient() => Play(SoundSource, first_emb);
}