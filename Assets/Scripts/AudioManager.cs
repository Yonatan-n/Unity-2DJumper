using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This value will control the master volume for the entire game (range 0 to 1)
    public float masterVolume = 0.1f;//1.0f;
    public AudioSource audioSource;
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
        SetMasterVolume(masterVolume);
    }
    public void EnemyIsHit()
    {
        audioSource.PlayOneShot(enemyIsHit);
    }

    public void GameOverSound()
    {
        audioSource.PlayOneShot(gameOver);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
    }

}