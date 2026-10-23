using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour
{
    public static Fader instance;

    public Animator animator;

    private const string FadeInTrigger = "FadeIn";
    private const string FadeOutTrigger = "FadeOut";

    [Header("Fade Settings")]
    [SerializeField] private float fadeInSpeed = 2f;
    [SerializeField] private float fadeOutSpeed = 2f;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn()
    {
        animator.SetTrigger(FadeInTrigger);
        yield return new WaitForSeconds(GetAnimationLength(FadeInTrigger) / fadeInSpeed);
    }



    public IEnumerator FadeOut()
    {
        animator.SetTrigger(FadeOutTrigger);
        yield return new WaitForSeconds(GetAnimationLength(FadeOutTrigger) / fadeOutSpeed);
    }

    private float GetAnimationLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }
}
