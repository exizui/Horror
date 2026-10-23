using UnityEngine;
using UnityEngine.SceneManagement;

public class AntiAbuse : MonoBehaviour
{
    public AudioClip sound;
    private AudioSource audioSource;
    private SceneLoader sceneLoader;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sound);
            sceneLoader.LoadScene("WrongEnd");
            //SceneManager.LoadScene(3);
        }
    }
}
