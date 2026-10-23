using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static Action OnLoadScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadScene(string name)
    {
        StartCoroutine(Load(name));
    }

    private IEnumerator Load(string name)
    {
        yield return Fader.instance.FadeOut();

        AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    
        while (!async.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(name);
        SceneManager.SetActiveScene(newScene);
    }

    public void Unload(GameObject caller)
    {
        StartCoroutine(UnloadRoutine(caller)); 
    }

    private IEnumerator UnloadRoutine(GameObject caller)
    {
        yield return Fader.instance.FadeOut();

        Scene scene = caller.scene;

        //LocationNavigator.Controller.LoadPrevLocation();
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);

            if (s != scene && s.isLoaded)
            {
                SceneManager.SetActiveScene(s);
                break;
            }
        }
        AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
        //PauseManager.instance.SetCanRestart(true);
        yield return Fader.instance.FadeIn();

    }

    public void LoadGame(string name)
    {
        StartCoroutine(Game(name));
    }

    private IEnumerator Game(string name)
    {
        yield return Fader.instance.FadeOut();
        SceneManager.LoadScene(name);
        yield return Fader.instance.FadeIn();
    }
}
