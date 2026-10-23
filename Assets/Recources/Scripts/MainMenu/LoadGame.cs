using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    private SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }
    public void Load()
    {
        //SceneManager.LoadScene(1);
        sceneLoader.LoadGame("Day");
        print("mnasdas");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
