using UnityEngine;

public class SoundManagerGun : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip reload1911_1;
    [SerializeField] AudioClip reload1911_2;
    [SerializeField] AudioClip reload1911_3;
    [SerializeField] AudioClip reload1911_4;
    [SerializeField] AudioClip reload1911_5;
    [SerializeField] AudioClip reload1911_6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    public void Reload1911_1()
    {
        // slide back, drop mag
        audioSource.PlayOneShot(reload1911_1);
    }
    public void Reload1911_2()
    {
        // drop mag
        audioSource.PlayOneShot(reload1911_2);

    }
    public void Reload1911_3()
    {
        // insert new mag
        audioSource.PlayOneShot(reload1911_3);
    }
    public void Reload1911_4()
    {
        // drop slide / cock slide
        audioSource.PlayOneShot(reload1911_4);
    }
    public void Reload1911_5()
    {
        // drop slide / cock slide
        audioSource.PlayOneShot(reload1911_5);
    }
    public void Reload1911_6()
    {
        // drop slide / cock slide
        audioSource.PlayOneShot(reload1911_6);
    }
}
