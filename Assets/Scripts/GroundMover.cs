using UnityEngine;

public class GroundMover : ParentAwareSingleton<GroundMover>
{
    [SerializeField] GameObject quarter;
    public float speed = 8;
    float backgroundHalfWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundHalfWidth = quarter.GetComponent<SpriteRenderer>().bounds.size.x * 2;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < -backgroundHalfWidth)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }
}
