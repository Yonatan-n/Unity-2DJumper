using UnityEngine;
using UnityEngine.Audio;

public class GunTinnitus : Singleton<GunTinnitus>
{
    [Header("References")]
    [SerializeField] private AudioSource tinnitusSource;
    [SerializeField] private AudioMixer mixer;

    [Header("Tinnitus Settings")]
    public float gainPerShot = 0.2f;
    public float maxVolume = 0.8f;
    public float decayRate = 0.25f;
    public float riseSpeed = 4f;
    [Header("World Muffle Settings")]
    public float muffledCutoff = 4000f;      // How muffled the world becomes
    public float normalCutoff = 22000f;      // Default full frequency
    public float muffleSpeed = 2f;
    [Header("Tinnitus Tone Shaping")]
    public float tinnitusLowpassCutoff = 6000f;  // soften harsh highs
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private float currentWorldCutoff = 22000f;
    private const string WorldLowpassParam = "WorldLowpass";
    private const string TinnitusLowpassParam = "TinnitusLowpass";

    private void Start()
    {
        currentWorldCutoff = normalCutoff;
        mixer.SetFloat(WorldLowpassParam, normalCutoff);
        mixer.SetFloat(TinnitusLowpassParam, tinnitusLowpassCutoff);
    }

    private void Update()
    {
        // -------- Volume Fade --------
        currentVolume = Mathf.MoveTowards(
            currentVolume,
            targetVolume,
            riseSpeed * Time.deltaTime
        );

        tinnitusSource.volume = currentVolume;

        // -------- Decay --------
        if (targetVolume > 0f)
        {
            targetVolume -= decayRate * Time.deltaTime;
            targetVolume = Mathf.Max(targetVolume, 0f);
        }

        // -------- World Muffle --------
        float targetCutoff = currentVolume > 0.01f ? muffledCutoff : normalCutoff;

        currentWorldCutoff = Mathf.Lerp(
            currentWorldCutoff,
            targetCutoff,
            Time.deltaTime * muffleSpeed
        );

        mixer.SetFloat(WorldLowpassParam, currentWorldCutoff);

        // -------- Optional Subtle Pitch Wobble --------
        // float wobble = Mathf.Sin(Time.time * 2f) * 0.01f;
        // tinnitusSource.pitch = 1f + wobble;
        float wobble = Mathf.Sin(Time.time * 2f) * 0.01f; // small sine
        float randomDrift = Random.Range(-0.002f, 0.002f); // very subtle jitter
        tinnitusSource.pitch = 1f + wobble + randomDrift;
    }

    public void TriggerTinnitus()
    {
        targetVolume += gainPerShot;
        targetVolume = Mathf.Min(targetVolume, maxVolume);
    }
}
