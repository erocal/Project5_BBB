using System;
using ToolBox.Pools;
using UnityEngine;
using static LevelStatus;

public class BallSpawner : MonoBehaviour
{

    public static BallSpawner Instance { get; private set; }
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private Vector3 spawnPos;

    [SerializeField] private GameObject curBall;

    public GameObject CurBall => curBall;

    public event Action<LevelState> OnStateChanged;

    [Header("球生成父物件")]
    [Tooltip("生成出來的球都會放到這個 Transform 底下")]
    [SerializeField] private Transform parentTransform;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 換場景不銷毀
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += BallSpawnerHandleLevelStateChanged;

    }

    private void BallSpawnerHandleLevelStateChanged(LevelStatus.LevelState state)
    {
        switch (state)
        {
            case LevelStatus.LevelState.Loading:
                break;

            case LevelStatus.LevelState.Ready:
                Reuse(spawnPos, Quaternion.identity);
                break;

            case LevelStatus.LevelState.Playing:

                break;

            case LevelStatus.LevelState.Cleared:

                break;

            case LevelStatus.LevelState.Failed:
                curBall = null;

                break;
        }
    }

    public GameObject Reuse(Vector3 position, Quaternion rotation)
    {
        curBall = ballPrefab.Reuse(position, rotation, parentTransform);
        curBall.GetComponent<BallManager>().StopBall();

        return curBall;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // 如果 spawnPos 是世界座標
        Gizmos.DrawSphere(spawnPos, 0.2f);

    }

    private void OnDisable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= BallSpawnerHandleLevelStateChanged;

    }

}
