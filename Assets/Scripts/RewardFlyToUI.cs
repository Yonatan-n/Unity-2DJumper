using System;
using System.Collections;
using UnityEngine;

public class RewardFlyToUI : MonoBehaviour
{
    float duration = 0.5f;          // Flight duration
    public float arcHeight = 100f;         // Arc height
    Vector2 scaleRange = new Vector2(0.8f, 2f); // Scaling at target
    public float rotationSpeed = 360f;     // Degrees per second, set 0 to disable
    float maxRandomDelay = 0.2f;    // Random delay per coin

    private RectTransform rect;
    private RectTransform target;
    private Camera worldCamera;
    private Canvas canvas;
    Vector3 worldPosition;
    private Action onCollected;
    private Action<RewardFlyToUI> returnToPool;

    private Coroutine running;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        worldCamera = Camera.main;
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

        rect.localScale = Vector3.one;
        running = StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        // Random start delay for burst effect
        if (maxRandomDelay > 0f)
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, maxRandomDelay));

        // Convert positions to canvas-local space
        Vector2 start;
        Vector2 end;
        RectTransform canvasRect = canvas.transform as RectTransform;
        var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, camera, out start);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(camera, target.position),
            camera, out end);

        rect.anchoredPosition = start;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Easing: ease-out cubic for smoother snap
            float easeT = 1 - Mathf.Pow(1 - t, 3);

            // Quadratic bezier for arc
            Vector2 mid = (start + end) / 2 + Vector2.up * arcHeight;
            Vector2 m1 = Vector2.Lerp(start, mid, easeT);
            Vector2 m2 = Vector2.Lerp(mid, end, easeT);
            rect.anchoredPosition = Vector2.Lerp(m1, m2, easeT);

            // Optional rotation
            if (rotationSpeed != 0f)
                rect.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        rect.anchoredPosition = end;

        // Scaling effect at target
        float scaleTime = 0.15f;
        float st = 0f;
        while (st < scaleTime)
        {
            st += Time.deltaTime;
            float s = Mathf.SmoothStep(scaleRange.x, scaleRange.y, st / scaleTime);
            rect.localScale = new Vector3(s, s, 1f);
            yield return null;
        }

        rect.localScale = Vector3.one;

        onCollected?.Invoke();
        returnToPool?.Invoke(this);
    }
}