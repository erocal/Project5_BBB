using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool destroyOldBallWhenSpawnNew = true;

    [Header("Launch Settings")]
    [SerializeField] private float launchSpeed = 12f;

    [Tooltip("水平前方左右隨機角度範圍。例如 60 代表 -60 到 +60 度。")]
    [SerializeField] private float randomHorizontalAngleRange = 60f;

    [Header("XR Input")]
    [Tooltip("建議拖 XRI LeftHand Interaction / Activate，通常是 Trigger。")]
    [SerializeField] private InputActionReference leftHandActivateAction;

    [Tooltip("建議拖 XRI RightHand Interaction / Activate，通常是 Trigger。")]
    [SerializeField] private InputActionReference rightHandActivateAction;

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
        if (rightHandActivateAction != null)
        {
            rightHandActivateAction.action.Enable();
            rightHandActivateAction.action.performed += OnRightHandActivatePerformed;
        }

        if (leftHandActivateAction != null)
        {
            leftHandActivateAction.action.Enable();
            leftHandActivateAction.action.performed += OnLeftHandActivatePerformed;
        }
    }

    private void OnDisable()
    {
        if (rightHandActivateAction != null)
        {
            rightHandActivateAction.action.performed -= OnRightHandActivatePerformed;
            rightHandActivateAction.action.Disable();
        }

        if (leftHandActivateAction != null)
        {
            leftHandActivateAction.action.performed -= OnLeftHandActivatePerformed;
            leftHandActivateAction.action.Disable();
        }
    }

    private void OnRightHandActivatePerformed(InputAction.CallbackContext context)
    {
        LaunchCurrentBall();
    }

    private void OnLeftHandActivatePerformed(InputAction.CallbackContext context)
    {
        SpawnBall();
    }

    public void SpawnBall()
    {

        if (destroyOldBallWhenSpawnNew && currentBall != null)
        {
            Destroy(currentBall.gameObject);
            currentBall = null;
        }

        currentBall = BallSpawner.Instance.Reuse(spawnPoint.position, spawnPoint.rotation).GetComponent<BallManager>();

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