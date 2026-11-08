using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GameOverManager))]
public class GameAudio : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip melcochaSound;
    [SerializeField] private AudioClip transferSound;
    [SerializeField] private StoneFollowAnchor stone;

    [Header("Volumen")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.4f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 0.85f;

    private GameOverManager gameOverManager;
    private AudioSource musicSource;
    private AudioSource effectsSource;

    private void Awake()
    {
        gameOverManager = GetComponent<GameOverManager>();

        // Dos canales: la música no corta los sonidos de recogida y transferencia.
        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        effectsSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
        musicSource.clip = backgroundMusic;

        effectsSource.playOnAwake = false;
        effectsSource.spatialBlend = 0f;
        effectsSource.volume = effectsVolume;
    }

    private void OnEnable()
    {
        if (stone != null) stone.StoneTransferred += OnStoneTransferred;
        if (gameOverManager != null) gameOverManager.GameOverStarted += OnGameOverStarted;
    }

    private void Start()
    {
        if (backgroundMusic != null) musicSource.Play();
    }

    public void PlayMelcocha()
    {
        if (gameOverManager != null && gameOverManager.IsGameOver) return;
        if (effectsSource != null && melcochaSound != null)
            effectsSource.PlayOneShot(melcochaSound);
    }

    private void OnStoneTransferred(Transform newHolder)
    {
        if (gameOverManager != null && gameOverManager.IsGameOver) return;
        if (effectsSource != null && transferSound != null)
            effectsSource.PlayOneShot(transferSound);
    }

    private void OnGameOverStarted()
    {
        if (musicSource != null) musicSource.Stop();
    }

    private void OnDisable()
    {
        if (stone != null) stone.StoneTransferred -= OnStoneTransferred;
        if (gameOverManager != null) gameOverManager.GameOverStarted -= OnGameOverStarted;
        if (musicSource != null) musicSource.Stop();
    }
}
