using System;
using ToolBox.Pools;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Launch Settings")]
    [SerializeField] private float launchSpeed = 12f;

    [Tooltip("水平前方左右隨機角度範圍。例如 60 代表 -60 到 +60 度。")]
    [SerializeField] private float randomHorizontalAngleRange = 60f;

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

        if(VRController.Instance == null)
            return ;

        if (VRController.Instance.RightHandActivateAction != null)
        {
            VRController.Instance.RightHandActivateAction.action.Enable();
            VRController.Instance.RightHandActivateAction.action.performed += BallSpawnerOnRightHandActivatePerformed;
        }

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

    private void BallSpawnerOnRightHandActivatePerformed(InputAction.CallbackContext context)
    {
        LaunchCurrentBall();
    }

    public void LaunchCurrentBall()
    {

        if (curBall == null)
        {
            Debug.LogWarning("場上目前沒有球，請先按 Grip / Select 生成球。");
            return;
        }

        Vector3 launchDirection = GetRandomHorizontalForwardDirection();


        BallSpawner.Instance.CurBall.GetComponent<BallManager>().Launch(launchDirection, launchSpeed);

        if (LevelStatus.Instance != null)
            LevelStatus.Instance.SetState(LevelStatus.LevelState.Playing);

    }

    private Vector3 GetRandomHorizontalForwardDirection()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude <= 0.001f)
        {
            forward = Vector3.forward;
        }

        forward.Normalize();

        float randomAngle = UnityEngine.Random.Range(
            -randomHorizontalAngleRange,
            randomHorizontalAngleRange
        );

        Vector3 randomDirection =
            Quaternion.AngleAxis(randomAngle, Vector3.up) * forward;

        return randomDirection.normalized;
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

        if (VRController.Instance.RightHandActivateAction != null)
        {
            VRController.Instance.RightHandActivateAction.action.performed -= BallSpawnerOnRightHandActivatePerformed;
            VRController.Instance.RightHandActivateAction.action.Disable();
        }

    }

}
