using ToolBox.Pools;
using UnityEngine;

public class SplitBallManager : EnvironmentObject
{

    [Header("Split Ball")]
    [SerializeField] private GameObject SplitBallVFX;

    [SerializeField] private float splitAngle = 30f;
    [SerializeField] private float spawnOffset = 0.2f;
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float cooldownTime = 2f;

    private float cooldownTimer = 0f;

    private void OnEnable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += SplitBallManagerHandleLevelStateChanged;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        cooldownTimer += Time.fixedDeltaTime;

    }

    private void SplitBallManagerHandleLevelStateChanged(LevelStatus.LevelState state)
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

                break;

            case LevelStatus.LevelState.Failed:

                this.gameObject.Release();

                break;
        }
    }

    protected override void OnHit(GameObject hitObject)
    {

        if (cooldownTimer < cooldownTime) { return; }

        base.OnHit(hitObject);

        SplitBallVFX.SetActive(true);

        SplitBall(hitObject);

        cooldownTimer = 0f;

        MusicManager.Instance.PlayItemGetAudio();

    }

    private void SplitBall(GameObject hitObject)
    {
        if (hitObject == null)
            return;

        Rigidbody originalBallRb = hitObject.GetComponent<Rigidbody>();

        if (originalBallRb == null)
        {
            Debug.LogWarning($"{hitObject.name} 沒有 Rigidbody，無法分裂球");
            return;
        }

        Vector3 originalVelocity = originalBallRb.velocity;

        if (originalVelocity.sqrMagnitude <= 0.01f)
        {
            originalVelocity = hitObject.transform.forward * minSpeed;
        }

        float speed = Mathf.Max(originalVelocity.magnitude, minSpeed);
        Vector3 forwardDir = originalVelocity.normalized;

        Vector3 leftDir = Quaternion.AngleAxis(-splitAngle, Vector3.up) * forwardDir;
        Vector3 rightDir = Quaternion.AngleAxis(splitAngle, Vector3.up) * forwardDir;

        SpawnExtraBall(transform.position, leftDir, speed);
        SpawnExtraBall(transform.position, rightDir, speed);
    }

    private void SpawnExtraBall(Vector3 centerPosition, Vector3 direction, float speed)
    {
        Vector3 spawnPosition = centerPosition + direction.normalized * spawnOffset;

        GameObject ball = BallSpawner.Instance.Reuse(spawnPosition, Quaternion.LookRotation(direction.normalized));

        ball.GetComponent<BallManager>().Launch(direction, speed);

    }

    private void OnDisable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= SplitBallManagerHandleLevelStateChanged;

    }

}