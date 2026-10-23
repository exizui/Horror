using UnityEngine;

public class Trigger : MonoBehaviour
{
    public AudioClip sound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sound);
        }
    }
}
