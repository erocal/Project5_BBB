using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource brickAudioSource;
    [SerializeField] private AudioSource wallAudioSource;
    [SerializeField] private AudioSource itemGetAudioSource;
    [SerializeField] private AudioSource levelClearAudioSource;

    private void Awake()
    {
        // 單例：如果已經有 MusicManager，就刪掉新的
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 換場景不銷毀
        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += MusicManagerHandleLevelStateChanged;

    }

    private void MusicManagerHandleLevelStateChanged(LevelStatus.LevelState state)
    {

        switch (state)
        {
            case LevelStatus.LevelState.Loading:
                
                break;

            case LevelStatus.LevelState.Ready:

                break;

            case LevelStatus.LevelState.Playing:

                break;

            case LevelStatus.LevelState.Cleared:
                PlayLevelClearAudio();
                break;

            case LevelStatus.LevelState.Failed:

                break;
        }

    }

    public void PlayBrickAudio()
    {
        brickAudioSource.Stop();
        brickAudioSource.Play();
    }

    public void PlayWallAudio()
    {
        wallAudioSource.Stop();
        wallAudioSource.Play();
    }

    public void PlayItemGetAudio()
    {
        //itemGetAudioSource.Stop();
        itemGetAudioSource.Play();
    }

    public void PlayLevelClearAudio()
    {
        levelClearAudioSource.Play();
    }

    private void OnDisable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= MusicManagerHandleLevelStateChanged;

    }

}