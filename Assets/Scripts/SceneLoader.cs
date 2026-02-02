using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.Rendering;
using System.Collections;
public class SceneLoader : Singleton<SceneLoader>
{
    private Animator animator;
    [SerializeField] AnimationClip FadeOutClip;
    [SerializeField] AnimationClip FadeInClip;

    void Start()
    {
        Instance.animator = GetComponent<Animator>();
    }
    static public void LoadSceneByName(string sceneName)
    {
        // SceneManager.LoadScene(sceneName);
        Instance.StartCoroutine(Instance.FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        Instance.animator.SetTrigger("FadeOut");
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
        Instance.animator.SetTrigger("FadeIn");
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
