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
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(FadeOutClip.length + 0.5f);
        // SceneManager.LoadScene(sceneName);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            // You can update a loading bar here using operation.progress (0 to 0.9)
            Debug.Log("Loading progress: " + operation.progress);
            yield return null; // Wait for the next frame
        }
        operation.allowSceneActivation = true;
        yield return null;
        // scene loaded
        // yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("FadeIn");
    }

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
