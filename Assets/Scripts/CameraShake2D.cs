using UnityEngine;
using System.Collections;

public class CameraShake2D : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.3f;      // How long the shake lasts
    public float magnitude = 0.2f;     // Maximum offset

    Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines(); // Stop previous shake if it's still running
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration; // 0 → 1
            float damper = 1.0f - progress;      // fades out

            // Recoil-like shake: mostly vertical/backward, slight horizontal
            float x = Random.Range(-0.02f, 0.02f) * magnitude * damper;
            float y = Random.Range(0.5f, 1f) * magnitude * damper; // push up/back
            float z = 0;

            transform.localPosition = originalPos + new Vector3(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos; // reset
    }
}
