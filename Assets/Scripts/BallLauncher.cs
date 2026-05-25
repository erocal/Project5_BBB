using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [Header("Ball Prefab")]
    [SerializeField] private BallManager ballPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool destroyOldBallWhenSpawnNew = true;

    [Header("Launch Settings")]
    [SerializeField] private float launchSpeed = 12f;

    [Tooltip("水平前方左右隨機角度範圍。例如 60 代表 -60 到 +60 度。")]
    [SerializeField] private float randomHorizontalAngleRange = 60f;

    [Header("XR Input")]
    [Tooltip("建議拖 XRI RightHand Interaction / Activate，通常是 Trigger。")]
    [SerializeField] private InputActionReference activateAction;

    [Tooltip("建議拖 XRI RightHand Interaction / Select，通常是 Grip。")]
    [SerializeField] private InputActionReference selectAction;

    private BallManager currentBall;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    private void OnEnable()
    {
        if (activateAction != null)
        {
            activateAction.action.Enable();
            activateAction.action.performed += OnActivatePerformed;
        }

        if (selectAction != null)
        {
            selectAction.action.Enable();
            selectAction.action.performed += OnSelectPerformed;
        }
    }

    private void OnDisable()
    {
        if (activateAction != null)
        {
            activateAction.action.performed -= OnActivatePerformed;
            activateAction.action.Disable();
        }

        if (selectAction != null)
        {
            selectAction.action.performed -= OnSelectPerformed;
            selectAction.action.Disable();
        }
    }

    private void OnActivatePerformed(InputAction.CallbackContext context)
    {
        LaunchCurrentBall();
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        SpawnBall();
    }

    public void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogWarning("BBBBallLauncher: 尚未指定 ballPrefab。");
            return;
        }

        if (destroyOldBallWhenSpawnNew && currentBall != null)
        {
            Destroy(currentBall.gameObject);
            currentBall = null;
        }

        currentBall = Instantiate(
            ballPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        currentBall.StopBall();
    }

    public void LaunchCurrentBall()
    {
        if (currentBall == null)
        {
            Debug.LogWarning("BBBBallLauncher: 場上目前沒有球，請先按 Grip / Select 生成球。");
            return;
        }

        Vector3 launchDirection = GetRandomHorizontalForwardDirection();
        currentBall.Launch(launchDirection, launchSpeed);
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

        float randomAngle = Random.Range(
            -randomHorizontalAngleRange,
            randomHorizontalAngleRange
        );

        Vector3 randomDirection =
            Quaternion.AngleAxis(randomAngle, Vector3.up) * forward;

        return randomDirection.normalized;
    }
}