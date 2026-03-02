using System;
using System.Collections;
using UnityEngine;

public class RewardFlyToUI : MonoBehaviour
{
    public float duration = 0.6f;
    public float arcHeight = 100f;

    private RectTransform rect;
    private RectTransform target;
    Vector3 worldPosition;
    Canvas canvas;
    Vector3 screenPos;
    private Action onCollected;
    private Action<RewardFlyToUI> returnToPool;

    private Coroutine running;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Initialize(
        Vector3 worldPosition,
        RectTransform target,
        Canvas canvas,
        Action onCollected,
        Action<RewardFlyToUI> returnToPool)
    {
        this.target = target;
        this.canvas = canvas;
        this.worldPosition = worldPosition;
        this.onCollected = onCollected;
        this.returnToPool = returnToPool;

        if (running != null)
            StopCoroutine(running);

        this.screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 canvasPos);

        rect.anchoredPosition = canvasPos;

        running = StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        Vector2 start;
        Vector2 end;
        var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        var canvasRect = canvas.transform as RectTransform;
        // set start
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, camera, out start);

        // Convert target position to canvas space, set end
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(camera, target.position), camera, out end);

        rect.anchoredPosition = start;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector2 mid = (start + end) / 2 + Vector2.up * arcHeight;
            Vector2 m1 = Vector2.Lerp(start, mid, t);
            Vector2 m2 = Vector2.Lerp(mid, end, t);
            rect.anchoredPosition = Vector2.Lerp(m1, m2, t);
            yield return null;
        }

        rect.anchoredPosition = end;
        onCollected?.Invoke();
        returnToPool?.Invoke(this);
    }
}