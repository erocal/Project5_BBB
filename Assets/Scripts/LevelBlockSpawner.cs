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

    private void Awake()
    {
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        int targetLevelId = GetLevelId(1);

        LevelIndexItem levelItem = allLevel.FirstOrDefault(fd => fd.id == targetLevelId);

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

        LevelLoader.LoadLevel(curLevel, parentTransform);

        parentTransform.position = targetBottomLeftPoint;
    }

    private int GetLevelId(int id)
    {
        return PlayerPrefs.GetInt("Level", id);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(targetBottomLeftPoint, 0.2f);

    }

}