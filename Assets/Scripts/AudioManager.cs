using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This value will control the master volume for the entire game (range 0 to 1)
    public float masterVolume = 0.1f;//1.0f;
    AudioSource audioSource;
    [SerializeField] AudioClip enemyIsHit;
    [SerializeField] AudioClip gameOver;
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

    }
    void Start()
    {
        Instance.audioSource = GetComponent<AudioSource>();
        Instance.audioSource.ignoreListenerPause = true;
        SetMasterVolume(masterVolume);
    }
    public void EnemyIsHit()
    {
        Instance.audioSource.PlayOneShot(enemyIsHit);
    }

    public void PlayGameOverSound()
    {
        Instance.audioSource.PlayOneShot(gameOver);
    }

    public void SetMasterVolume(float volume)
    {
        Instance.masterVolume = volume;
        AudioListener.volume = masterVolume;
    }

}