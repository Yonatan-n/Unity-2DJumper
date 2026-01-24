using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This value will control the master volume for the entire game (range 0 to 1)
    public float masterVolume = 0.1f;//1.0f;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetMasterVolume(masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
    }

    public void Reload1911_1()
    {
        // slide back
    }
    public void Reload1911_2()
    {
        // drop mag
    }
    public void Reload1911_3()
    {
        // insert new mag
    }
    public void Reload1911_4()
    {
        // drop slide / cock slide
    }
}