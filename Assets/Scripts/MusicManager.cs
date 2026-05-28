using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource brickAudioSource;
    [SerializeField] private AudioSource itemGetAudioSource;

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

    public void PlayBrickAudio()
    {
        brickAudioSource.Stop();
        brickAudioSource.Play();
    }

    public void PlayItemGetAudio()
    {
        itemGetAudioSource.Stop();
        itemGetAudioSource.Play();
    }

}