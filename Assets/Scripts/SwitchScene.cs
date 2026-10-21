
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public AudioClip doorSlamClip;
    public AudioClip first_enb; // сюда перетащишь клип в инспекторе

    bool used;
    float delay = 1.5f;

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
        Debug.Log("AudioSource: " + (src != null ? src.name : "НЕ НАЙДЕН"));
        Debug.Log("Clip: " + (doorSlamClip != null ? doorSlamClip.name : "НЕ НАЗНАЧЕН"));


        yield return new WaitForSeconds(delay);
        if (src != null && doorSlamClip != null)
            src.PlayOneShot(doorSlamClip);
            src.PlayOneShot(first_enb);

        SceneManager.sceneLoaded += OnSceneLoaded;
        yield return SceneManager.LoadSceneAsync(1);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        if (player.GetComponent<TorchLighter>() == null)
            player.AddComponent<TorchLighter>();
    }
}