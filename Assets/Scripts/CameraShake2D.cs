using UnityEngine;
using System.Collections;

public class CameraShake2D : MonoBehaviour
{
    [Header("Shake Settings")]

    public float duration = 0.1f;        // How long the recoil lasts
    public float positionMagnitude = 0.08f; // How far the camera moves
    public float rotationMagnitude = 2f;    // How much it tilts
    public float horizontalShake = 0.02f;   // Small x-axis nudge
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Awake()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    public void Shake()
    {
        StopAllCoroutines(); // Stop previous shake if it's still running
        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;          // 0 → 1
            float damper = 1f - progress;                 // fades out

            // Smooth positional recoil using sin curve (up then down)
            float yOffset = Mathf.Sin(progress * Mathf.PI) * positionMagnitude;
            float xOffset = Random.Range(-horizontalShake, horizontalShake) * damper;

            transform.localPosition = originalPos + new Vector3(xOffset, yOffset, 0);

            // Small rotational tilt (z-axis) for a punchy feel
            float zRot = Random.Range(-rotationMagnitude, rotationMagnitude) * damper;
            transform.localRotation = Quaternion.Euler(0, 0, zRot);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }

}