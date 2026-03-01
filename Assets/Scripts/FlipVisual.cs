using UnityEngine;

public class FlipVisual : MonoBehaviour
{
    public float flipDuration = 0.25f;

    private float remainingRotation;
    private float rotationSpeed;

    void Start()
    {
        rotationSpeed = 360f / flipDuration;
    }

    void Update()
    {
        if (remainingRotation <= 0f) return;

        float step = rotationSpeed * Time.deltaTime;
        float rotateAmount = Mathf.Min(step, remainingRotation);

        transform.Rotate(0f, 0f, rotateAmount);
        remainingRotation -= rotateAmount;

        if (remainingRotation <= 0f)
        {
            // Snap cleanly to nearest 360
            float snapped = Mathf.Round(transform.eulerAngles.z / 360f) * 360f;
            transform.rotation = Quaternion.Euler(0f, 0f, snapped);
        }
    }

    public void TriggerFlip()
    {
        remainingRotation += 360f;
    }
}