using Unity.Mathematics;
using UnityEngine;

public class GroundMover : ParentAwareSingleton<GroundMover>
{
    [SerializeField] Transform childA;
    [SerializeField] Transform childB;
    public float speed = 14f;
    private float maxSpeed = 28f;
    [SerializeField] float step = 2f;
    float startX, childWidth;

    public void IncreaseSpeed()
    {
        speed = Mathf.Min(speed + step, maxSpeed);
    }
    void Start()
    {
        // Width of ONE child (which contains 2 tiles)
        childWidth = childA.GetComponentInChildren<SpriteRenderer>().bounds.size.x * 2f;
        startX = transform.position.x;
    }

    void Update()
    {
        float offset = Mathf.Repeat(Time.time * speed, childWidth);

        // Move children based on offset
        childA.position = new Vector3(startX - offset, childA.position.y, childA.position.z);
        childB.position = new Vector3(startX - offset + childWidth, childB.position.y, childB.position.z);
    }
}
