using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicCarousel : MonoBehaviour
{
    public enum PlayMode
    {
        Sequential,
        Random
    }

    [Header("Music List")]
    [SerializeField] private AudioClip[] musicClips;

    [Header("Playback Settings")]
    [SerializeField] private PlayMode playMode = PlayMode.Sequential;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopPlaylist = true;
    [SerializeField] private bool avoidRepeatInRandom = true;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float targetVolume = 0.6f;

    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("Gap Between Songs")]
    [SerializeField] private float delayBetweenSongs = 0.5f;

    private AudioSource audioSource;
    private int currentIndex = -1;
    private Coroutine playlistCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0f;
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
        }

        playlistCoroutine = StartCoroutine(PlayPlaylistRoutine());
    }

    public void Stop()
    {
        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }

        StartCoroutine(StopWithFadeRoutine());
    }

    public void SkipToNext()
    {
        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
        }

        playlistCoroutine = StartCoroutine(SkipRoutine());
    }

    private IEnumerator PlayPlaylistRoutine()
    {
        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("[BackgroundMusicCarousel] 沒有設定任何背景音樂。");
            yield break;
        }

        do
        {
            int nextIndex = GetNextIndex();
            currentIndex = nextIndex;

            AudioClip clip = musicClips[currentIndex];

            if (clip == null)
            {
                Debug.LogWarning($"[BackgroundMusicCarousel] 第 {currentIndex} 首音樂是空的，已跳過。");
                yield return null;
                continue;
            }

            yield return PlayClipRoutine(clip);
            yield return new WaitForSeconds(delayBetweenSongs);

        } while (loopPlaylist);
    }

    private IEnumerator PlayClipRoutine(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeVolumeRoutine(targetVolume, fadeInDuration);

        while (audioSource.isPlaying && audioSource.time < clip.length - fadeOutDuration)
        {
            yield return null;
        }

        yield return FadeVolumeRoutine(0f, fadeOutDuration);

        audioSource.Stop();
        audioSource.clip = null;
    }

    private IEnumerator SkipRoutine()
    {
        yield return FadeVolumeRoutine(0f, fadeOutDuration);

        audioSource.Stop();
        audioSource.clip = null;

        playlistCoroutine = StartCoroutine(PlayPlaylistRoutine());
    }

    private IEnumerator StopWithFadeRoutine()
    {
        yield return FadeVolumeRoutine(0f, fadeOutDuration);

        audioSource.Stop();
        audioSource.clip = null;
    }

    private IEnumerator FadeVolumeRoutine(float target, float duration)
    {
        if (duration <= 0f)
        {
            audioSource.volume = target;
            yield break;
        }

        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, target, timer / duration);
            yield return null;
        }

        audioSource.volume = target;
    }

    private int GetNextIndex()
    {
        if (musicClips.Length == 1)
        {
            return 0;
        }

        switch (playMode)
        {
            case PlayMode.Random:
                return GetRandomIndex();

            case PlayMode.Sequential:
            default:
                return (currentIndex + 1) % musicClips.Length;
        }
    }

    private int GetRandomIndex()
    {
        int nextIndex;

        do
        {
            nextIndex = Random.Range(0, musicClips.Length);
        }
        while (avoidRepeatInRandom && nextIndex == currentIndex);

        return nextIndex;
    }
}