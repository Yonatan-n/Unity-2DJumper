using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackgroundFade : MonoBehaviour
{
    SpriteRenderer[] renderers;
    Coroutine fadeRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Runner")
        {
            // kind of a hack, to leave main menu alone
            ScaleBackground();
        }
    }

    public Coroutine Fade(float from, float to, float duration)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(from, to, duration));
        return fadeRoutine;
    }

    IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            SetAlpha(Mathf.Lerp(from, to, t / duration));
            t += Time.deltaTime;
            yield return null;
        }

        SetAlpha(to);
    }

    public void StopFade()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
    }

    void SetAlpha(float alpha)
    {
        foreach (var sr in renderers)
        {
            if (sr == null) continue; // safety
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    // scale
    void ScaleBackground()
    {
        SpriteRenderer main = renderers[0];
        if (main == null || main.sprite == null) return;
        Camera cam = Camera.main;
        float worldScreenHeight = cam.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * cam.aspect;
        // Use sprite bounds (not renderer bounds)
        float spriteWidth = main.sprite.bounds.size.x - 2f;
        float spriteHeight = main.sprite.bounds.size.y;
        float scaleX = (worldScreenWidth / spriteWidth);
        float scaleY = worldScreenHeight / spriteHeight;
        float totalScale = Mathf.Max(scaleX, scaleY);
        transform.localScale = new Vector3(totalScale, totalScale, 1f);

        // move background image 25% higher, to show the sky on mobile too
        float screenHeight = cam.orthographicSize * 2f;   // total visible height
        float offset = screenHeight * 0.25f;              // 25% of screen height
        transform.position += new Vector3(0f, -offset, 0f);
    }

}

