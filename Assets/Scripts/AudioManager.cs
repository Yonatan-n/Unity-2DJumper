using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // This value will control the master volume for the entire game (range 0 to 1)
    public AudioSource audioSource;
    private readonly string MixerMasterVolume = "MasterVolume";
    [SerializeField] AudioClip enemyIsHit;
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip coins;
    [SerializeField] AudioClip shopNo;
    [SerializeField] AudioClip shopYes;
    [SerializeField] AudioClip shoot;
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
        SetMasterVolume(null);
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
    // for volume level change check
    public void Shoot()
    {
        // not oneShot
        audioSource.clip = shoot;
        audioSource.Play();
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

    public void SetMasterVolume(float? volume)
    {
        volume ??= PlayerData.GetFloatById(PlayerData.MasterVolume, 0.5f);
        var masterVolume = (float)volume;
        float dB = masterVolume <= 0.0001f ? -80f : Mathf.Log10(masterVolume) * 20f;
        mixer.SetFloat(MixerMasterVolume, dB);
        float val;
        bool success = mixer.GetFloat(MixerMasterVolume, out val);
        Debug.Log("MasterVolume current dB: " + val + " | SetFloat success? " + success);
    }

}