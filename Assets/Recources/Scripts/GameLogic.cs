using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private static GameLogic mInstance;
    public static GameLogic Instance => mInstance;

    [HideInInspector]
    public TextMeshProUGUI Popuptext;

    [Header("Подсказка (находятся сами при загрузке сцены)")]
    public TextMeshProUGUI PressButton;
    public GameObject pressE;

    private string clueText = "Press E";
    private string text;
    private float waitbetchar = 0.13f;
    private Coroutine textRoutine;

    public Animator faderAnimator;

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        mInstance = this;
        DontDestroyOnLoad(gameObject);

        FindSceneObjects();
    }

    private void Start()
    {
        FadeEnable();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneObjects();
        ShowClue(false);
    }

    // Ссылки на объекты старой сцены умирают при загрузке, поэтому ищем заново
    private void FindSceneObjects()
    {
        pressE = null;
        PressButton = null;

        foreach (Canvas canvas in FindObjectsByType<Canvas>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (Transform t in canvas.GetComponentsInChildren<Transform>(true))
            {
                if (t.name != "pressE") continue;

                pressE = t.gameObject;
                PressButton = t.GetComponentInChildren<TextMeshProUGUI>(true);

                // Запоминаем исходный текст, чтобы всегда уметь его вернуть
                if (PressButton != null && !string.IsNullOrEmpty(PressButton.text))
                    clueText = PressButton.text;

                return;
            }
        }
    }

    #region Всплывающий текст

    public void ShowMessage(string text)
    {
        this.text = text;

        if (textRoutine != null) StopCoroutine(textRoutine);
        textRoutine = StartCoroutine(TextCoroutine());
    }

    private IEnumerator TextCoroutine()
    {
        if (Popuptext != null)
            Popuptext.text = "";

        foreach (char abc in text)
        {
            if (Popuptext != null)
                Popuptext.text += abc;

            yield return new WaitForSeconds(waitbetchar);
        }

        yield return new WaitForSeconds(3f);

        if (Popuptext != null)
            Popuptext.text = "";

        textRoutine = null;
    }

    #endregion

    #region Подсказка "Press E"

    // Главный метод: его вызывает Interactor каждый кадр
    public void ShowClue(bool show)
    {
        if (pressE == null) return;

        if (show && PressButton != null)
        {
            PressButton.enabled = true;

            // Если кто-то стёр текст, возвращаем исходный
            if (string.IsNullOrEmpty(PressButton.text))
                PressButton.text = clueText;
        }

        if (pressE.activeSelf != show)
            pressE.SetActive(show);
    }

    // Подсказка с произвольным текстом (например "Открыть")
    public void ShowPressButton(string text)
    {
        if (pressE == null) return;

        if (PressButton != null)
        {
            PressButton.enabled = true;
            PressButton.text = text;
        }

        pressE.SetActive(true);
    }

    // Теперь просто скрывает подсказку и ничего не стирает
    public void HidePressButton()
    {
        ShowClue(false);
    }

    #endregion


    private const string FadeInTrigger = "FadeIn";
    [SerializeField] private float fadeInSpeed = 2f;
    public void FadeEnable()
    {
        StartCoroutine(FadeIn());
    }
    public IEnumerator FadeIn()
    {
        faderAnimator.SetTrigger(FadeInTrigger);
        yield return new WaitForSeconds(GetAnimationLength(FadeInTrigger) / fadeInSpeed);
    }
    private float GetAnimationLength(string clipName)
    {
        foreach (var clip in faderAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }
}