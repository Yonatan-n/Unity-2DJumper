using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // This value will control the master volume for the entire game (range 0 to 1)
    [Range(0, 1)] public float masterVolume = 0.1f;//1.0f;
    [Range(0, 1)] public float sfxVolume = 0.1f;//1.0f;
    AudioSource audioSource;
    private readonly string MixerMasterVolume = "MasterVolume";
    [SerializeField] AudioClip enemyIsHit;
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip coins;
    [SerializeField] AudioClip shopNo;
    [SerializeField] AudioClip shopYes;
    [SerializeField] AudioMixer mixer;


    public bool IsInitialized { get; private set; }
    protected override void Awake()
    {
        base.Awake(); // must be first
        if (Instance != this)
            return;

        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }
    void Start()
    {
        SetMasterVolume(masterVolume);
        IsInitialized = true; // last line
    }

    public void EnemyIsHit()
    {
        audioSource.PlayOneShot(enemyIsHit);
    }

    public void ShopNoMoney()
    {
        audioSource.PlayOneShot(shopNo);
    }
    public void ShopYes()
    {
        audioSource.PlayOneShot(shopYes);
    }

    public void PlayGameOverSound()
    {
        if (gameOver == null)
        {
            Debug.LogWarning("GameOver clip is not assigned!");
            return;
        }
        audioSource.PlayOneShot(gameOver);
    }

    public void CoinPickUp()
    {
        audioSource.PlayOneShot(coins);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        float dB = masterVolume <= 0.0001f ? -80f : Mathf.Log10(masterVolume) * 20f;
        mixer.SetFloat(MixerMasterVolume, dB);
        float val;
        bool success = mixer.GetFloat(MixerMasterVolume, out val);
        Debug.Log("MasterVolume current dB: " + val + " | SetFloat success? " + success);
    }

}