using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using System.Collections;
public class SceneLoader : Singleton<SceneLoader>
{
    private Animator animator;
    [SerializeField] AnimationClip FadeOutClip;
    [SerializeField] AnimationClip FadeInClip;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void LoadSceneByName(string sceneName)
    {
        // StartCoroutine(FadeOut(sceneName));
        SceneManager.LoadScene(sceneName);
    }


    IEnumerator FadeOut(string sceneName)
    {
#if !UNITY_EDITOR
            animator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(FadeOutClip.length + 0.5f);
#endif

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            Debug.Log("Loading progress: " + operation.progress);
            yield return null;
        }
        operation.allowSceneActivation = true;
        yield return null;

#if !UNITY_EDITOR
            animator.SetTrigger("FadeIn");
#endif
    }

    //     IEnumerator FadeOut(string sceneName)
    //     {
    // #if !UNITY_EDITOR
    //     animator.SetTrigger("FadeOut");
    //     yield return new WaitForSeconds(FadeOutClip.length + 0.5f);
    // #endif

    //         AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    //         operation.allowSceneActivation = false;
    //         while (operation.progress < 0.9f)
    //         {
    //             Debug.Log("Loading progress: " + operation.progress);
    //             yield return null;
    //         }
    //         operation.allowSceneActivation = true;
    //         yield return null;

    // #if !UNITY_EDITOR
    //     animator.SetTrigger("FadeIn");
    // #endif
    //     }

    public void JustFadeOut()
    {
        StartCoroutine(FadeOutEnum());

    }
    public IEnumerator FadeOutEnum()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(FadeOutClip.length + 0.5f);
    }
    public void JustFadeIn()
    {
        StartCoroutine(FadeInEnum());

    }
    public IEnumerator FadeInEnum()
    {
        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(FadeInClip.length + 0.2f);
    }

    static public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    static public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    static public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
