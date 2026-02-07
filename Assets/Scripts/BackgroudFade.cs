using System.Collections;
using UnityEngine;
public class BackgroundFade : MonoBehaviour
{
    SpriteRenderer[] renderers;
    Coroutine fadeRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
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
}
