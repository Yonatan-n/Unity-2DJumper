using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This value will control the master volume for the entire game (range 0 to 1)
    public float masterVolume = 0.1f;//1.0f;

    void Start()
    {
        SetMasterVolume(masterVolume);
    }

    // A public method to be called by a UI slider or other scripts
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
    }
}