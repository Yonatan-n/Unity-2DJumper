using System.Collections;
using UnityEngine;
public class BackgroundFade : MonoBehaviour
{
    SpriteRenderer[] renderers;
    Coroutine fadeRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        ScaleBackground();
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
        float spriteWidth = main.sprite.bounds.size.x;
        float spriteHeight = main.sprite.bounds.size.y;
        float scaleX = worldScreenWidth / spriteWidth;
        float scaleY = worldScreenHeight / spriteHeight;
        float totalScale = Mathf.Max(scaleX, scaleY);
        transform.localScale = new Vector3(totalScale, totalScale, 1f);
    }

}

