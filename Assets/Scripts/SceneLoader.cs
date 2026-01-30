using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.Rendering;
using System.Collections;
public class SceneLoader : Singleton<SceneLoader>
{
    private Animator animator;
    void Start()
    {
        Instance.animator = GetComponent<Animator>();
    }
    static public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // Instance.StartCoroutine(Instance.FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        Instance.animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        // SceneManager.LoadScene(sceneName);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            // You can update a loading bar here using operation.progress (0 to 0.9)
            Debug.Log("Loading progress: " + Mathf.Clamp01(operation.progress / 0.9f));
            yield return null; // Wait for the next frame
        }
        // scene loaded
        yield return new WaitForSeconds(0.5f);
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
