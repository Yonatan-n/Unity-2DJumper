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
    [SerializeField] AudioClip reloadAK_1;
    [SerializeField] AudioClip reloadAK_2;
    [SerializeField] AudioClip reloadAK_3;
    [SerializeField] AudioClip reloadMP3_1;
    [SerializeField] AudioClip reloadMP3_2;
    [SerializeField] AudioClip reloadMP3_3;

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

    public void ReloadAK_1()
    {
        audioSource.PlayOneShot(reloadAK_1);
    }
    public void ReloadAK_2()
    {
        audioSource.PlayOneShot(reloadAK_2);
    }
    public void ReloadAK_3()
    {
        audioSource.PlayOneShot(reloadAK_3);
    }
    public void ReloadMP3_1()
    {
        audioSource.PlayOneShot(reloadMP3_1);
    }
    public void ReloadMP3_2()
    {
        audioSource.PlayOneShot(reloadMP3_2);
    }
    public void ReloadMP3_3()
    {
        audioSource.PlayOneShot(reloadMP3_3);
    }
}
