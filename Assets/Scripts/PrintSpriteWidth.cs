using UnityEngine;

public class PrintSpriteWidth : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float width = sr.bounds.size.x;
        Debug.Log("Sprite width in world units: " + width);
    }
}
