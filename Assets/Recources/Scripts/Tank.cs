using UnityEngine;

public class Tank : MonoBehaviour
{
    public AudioClip sound;
    private AudioSource audioSource;
    public Animator animator;
    private Collider colider;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        colider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(sound);
            animator.SetTrigger("Entry");
            colider.enabled = false;
            //выключил колайдер а не удалил обьект
            //чтобы звук полностью проигрался
        }
    }
}
