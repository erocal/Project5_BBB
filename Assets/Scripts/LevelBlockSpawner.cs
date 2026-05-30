using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using ThirdPixelGames.LevelBuilder;
using UnityEngine;

public class LevelBlockSpawner : MonoBehaviour
{
    [Header("所有可載入的關卡")]
    [Tooltip("A list of all levels that can be loaded")]
    [SerializeField] private List<LevelIndexItem> allLevel;

    [Header("目前生成的關卡")]
    [HideInInspector] public Level curLevel;

    [Header("關卡生成父物件")]
    [Tooltip("LevelLoader 生成出來的所有物件都會放到這個 Transform 底下")]
    [SerializeField] private Transform parentTransform;

    [Header("關卡左下角對齊的位置")]
    [SerializeField] private Vector3 targetBottomLeftPoint;

    [Header("關卡沉降動畫")]
    [SerializeField] private float dropHeight = 5f;
    [SerializeField] private float dropDuration = 1.2f;
    [SerializeField] private Ease dropEase = Ease.OutCubic;

    private Tween dropTween;

    private void Start()
    {

        LoadCurrentLevel(1);
    }

    private void OnEnable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += LevelBlockSpawnerHandleLevelStateChanged;

    }

    private void LevelBlockSpawnerHandleLevelStateChanged(LevelStatus.LevelState state)
    {
        switch (state)
        {
            case LevelStatus.LevelState.Loading:
                LoadCurrentLevel(1);
                break;

            case LevelStatus.LevelState.Ready:
                
                break;

            case LevelStatus.LevelState.Playing:
                
                break;

            case LevelStatus.LevelState.Cleared:
                
                break;

            case LevelStatus.LevelState.Failed:

                LevelCounter.Instance.ResetBallCount();
                LevelCounter.Instance.ResetBrickCount();

                break;
        }
    }

    public void LoadCurrentLevel(int targetLevelId)
    {

        LevelIndexItem levelItem = allLevel.FirstOrDefault(fd => fd.id == GetLevelId(targetLevelId));

        if (levelItem.level == null)
        {
            Debug.LogError($"[LevelBlockSpawner] 找不到 Level ID：{targetLevelId}，或該 Level 尚未設定。");
            return;
        }

        if (parentTransform == null)
        {
            Debug.LogError("[LevelBlockSpawner] 尚未指定 parentTransform。");
            return;
        }

        if (targetBottomLeftPoint == null)
        {
            Debug.LogError("[LevelBlockSpawner] 尚未指定 targetBottomLeftPoint。");
            return;
        }

        curLevel = levelItem.level;

        parentTransform.position = Vector3.zero;

        LevelLoader.LoadLevel(curLevel, parentTransform);

        PlayDropAnimation();

        LevelStatus.Instance.SetState(LevelStatus.LevelState.Ready);

    }

    private void PlayDropAnimation()
    {
        dropTween?.Kill();

        Vector3 startPosition = targetBottomLeftPoint + Vector3.up * dropHeight;

        parentTransform.position = startPosition;

        dropTween = parentTransform
            .DOMove(targetBottomLeftPoint, dropDuration)
            .SetEase(dropEase);
    }

    private int GetLevelId(int id)
    {
        return PlayerPrefs.GetInt("Level", id);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetBottomLeftPoint, 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(targetBottomLeftPoint + Vector3.up * dropHeight, 0.2f);

        Gizmos.DrawLine(
            targetBottomLeftPoint + Vector3.up * dropHeight,
            targetBottomLeftPoint
        );
    }

    private void OnDestroy()
    {
        dropTween?.Kill();
    }

    private void OnDisable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= LevelBlockSpawnerHandleLevelStateChanged;

    }

}