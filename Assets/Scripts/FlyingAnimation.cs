using UnityEngine;

public class FlyingAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform floatRoot;
    [SerializeField] private float width = 0.5f;   // horizontal range
    [SerializeField] private float height = 0.3f;  // vertical range
    [SerializeField] private float speed = 1f;
    [SerializeField] private float tiltAmount = 5f;

    private Vector3 baseLocalPos;
    private float offset;

    private void Awake()
    {
        baseLocalPos = floatRoot.localPosition;
        // prevents multiple enemies moving identically
        offset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float t = (Time.time + offset) * speed;
        float x = Mathf.Sin(t) * width;
        float y = Mathf.Sin(t * 2f) * height; // creates ∞-like motion
        floatRoot.localPosition = baseLocalPos + new Vector3(x, y, 0f);
        float tilt = Mathf.Sin(t) * tiltAmount;
        floatRoot.localRotation = Quaternion.Euler(0f, 0f, -tilt);
    }
}