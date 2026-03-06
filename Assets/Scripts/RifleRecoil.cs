using UnityEngine;

public class RifleRecoil : MonoBehaviour
{
    [Header("Rotation Recoil (Z axis)")]
    public float rotationKickPerShot = 3f;
    public float maxRotationAngle = 90f;
    public float rotationRecoverySpeed = 4f;
    public float rotationDamping = 0.85f;

    [Header("Position Kickback")]
    public float kickbackX = 0.1f;
    public float kickbackY = 0.05f;
    public float maxKickback = 0.5f;
    public float positionRecoverySpeed = 6f;
    public float positionDamping = 0.80f;

    private float currentRotation = 0f;
    private float rotationVelocity = 0f;
    private Vector3 currentPositionOffset = Vector3.zero;
    private Vector3 positionVelocity = Vector3.zero;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private bool isShooting = false;

    void Awake()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // ── Always apply velocity ──────────────────────────
        currentRotation += rotationVelocity;
        currentRotation = Mathf.Clamp(currentRotation, 0f, maxRotationAngle);

        currentPositionOffset += positionVelocity;
        currentPositionOffset.x = Mathf.Clamp(currentPositionOffset.x, -maxKickback, maxKickback);
        currentPositionOffset.y = Mathf.Clamp(currentPositionOffset.y, -maxKickback, maxKickback);

        // ── Recovery only when not shooting ───────────────
        if (!isShooting)
        {
            rotationVelocity *= rotationDamping;
            rotationVelocity -= currentRotation * rotationRecoverySpeed * Time.deltaTime;

            positionVelocity *= positionDamping;
            positionVelocity -= currentPositionOffset * positionRecoverySpeed * Time.deltaTime;
        }
        else
        {
            rotationVelocity *= 0.6f;
            positionVelocity *= 0.6f;
        }

        // ── Apply ─────────────────────────────────────────
        transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, currentRotation);
        transform.localPosition = originalPosition + currentPositionOffset;
    }

    public void ApplyRecoil()
    {
        isShooting = true;
        rotationVelocity += rotationKickPerShot;
        positionVelocity += new Vector3(kickbackX, kickbackY, 0f);
    }

    public void ResetRecoil()
    {
        isShooting = false;
    }

    public void HardReset()
    {
        currentRotation = 0f;
        rotationVelocity = 0f;
        currentPositionOffset = Vector3.zero;
        positionVelocity = Vector3.zero;
        isShooting = false;
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
    }
}