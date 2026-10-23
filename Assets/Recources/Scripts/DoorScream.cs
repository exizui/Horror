using System.Collections;
using UnityEngine;

public class DoorScream : MonoBehaviour
{
    public AudioSource soundSource; // можно назначить вручную
    public AudioClip door_slam;
    public AudioClip door_scrip;

    private void Awake()
    {
        if (soundSource == null)
            soundSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        yield return null; // ждём один кадр, пока сцена и игрок встанут на место
        soundSource.PlayOneShot(door_scrip);
        yield return new WaitForSeconds(2f);
        soundSource.PlayOneShot(door_slam);
    }
}
