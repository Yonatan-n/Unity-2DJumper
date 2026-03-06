using UnityEngine;

public class RifleRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilKickStrength = 8f;
    public float maxRecoilAngle = 25f;

    [Header("Recovery Settings")]
    public float recoverySpeed = 4f;
    public float damping = 0.85f;

    [Header("Horizontal Drift (optional)")]
    public float horizontalDriftStrength = 1f;  // 0 to disable

    private float verticalRecoil = 0f;
    private float horizontalRecoil = 0f;
    private float verticalVelocity = 0f;
    private float horizontalVelocity = 0f;
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        // Vertical spring recovery
        verticalVelocity += -verticalRecoil * recoverySpeed * Time.deltaTime;
        verticalVelocity *= damping;
        verticalRecoil += verticalVelocity * Time.deltaTime;
        verticalRecoil = Mathf.Clamp(verticalRecoil, 0f, maxRecoilAngle);

        // Horizontal spring recovery (drifts back to 0)
        horizontalVelocity += -horizontalRecoil * recoverySpeed * Time.deltaTime;
        horizontalVelocity *= damping;
        horizontalRecoil += horizontalVelocity * Time.deltaTime;

        transform.localRotation = originalRotation
            * Quaternion.Euler(-verticalRecoil, horizontalRecoil, 0f);
    }

    public void ApplyRecoil()
    {
        // Vertical kick — stacks with each shot
        verticalVelocity += recoilKickStrength;

        // Small random horizontal drift per shot
        float drift = Random.Range(-horizontalDriftStrength, horizontalDriftStrength);
        horizontalVelocity += drift;
    }

    public void ResetRecoil()
    {
        // Call this when trigger is released to snap recovery
        verticalVelocity = 0f;
        horizontalVelocity = 0f;
    }
}