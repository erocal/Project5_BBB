using System.Collections.Generic;
using UnityEngine;
using ToolBox.Pools;
using DG.Tweening;

public class LevelBlockSpawner : MonoBehaviour
{
    [Header("方塊 Prefab，請放入紅、藍、黃、綠等物件池 Prefab")]
    [SerializeField] private List<GameObject> blockPrefabs = new List<GameObject>();

    [Header("生成中心點")]
    [SerializeField] private Vector3 centerPosition = new Vector3(0f, 1f, 15f);

    [Header("方塊群尺寸：X * Y * Z")]
    [SerializeField] private Vector3Int gridSize = new Vector3Int(6, 12, 5);

    [Header("方塊間距")]
    [SerializeField] private Vector3 spacing = new Vector3(1.1f, 1.1f, 1.1f);

    [Header("是否在 Start 自動生成")]
    [SerializeField] private bool generateOnStart = true;

    [Header("DOTween 生成推進動畫")]
    [SerializeField] private bool useSpawnMoveAnimation = true;

    [Tooltip("方塊會先生成在目標點 Z 軸再多這個距離的位置，再推進到目標點")]
    [SerializeField] private float spawnForwardOffsetZ = 5f;

    [SerializeField] private float spawnMoveDuration = 0.6f;

    [SerializeField] private Ease spawnMoveEase = Ease.OutBack;

    [Header("是否讓每顆方塊有一點點延遲，做出波浪推進感")]
    [SerializeField] private bool useStaggerDelay = true;

    [SerializeField] private float staggerDelayPerZLayer = 0.04f;

    private readonly List<GameObject> spawnedBlocks = new List<GameObject>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateBlocks();
        }
    }

    [ContextMenu("Generate Blocks")]
    public void GenerateBlocks()
    {
        if (blockPrefabs == null || blockPrefabs.Count == 0)
        {
            Debug.LogWarning("[LevelBlockSpawner] 尚未指定任何方塊 Prefab。");
            return;
        }

        ClearSpawnedBlocks();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 targetPosition = GetCenteredPosition(x, y, z);

                    GameObject selectedPrefab = GetRandomBlockPrefab();
                    if (selectedPrefab == null)
                        continue;

                    Vector3 startPosition = targetPosition;

                    if (useSpawnMoveAnimation)
                    {
                        startPosition += new Vector3(0f, 0f, spawnForwardOffsetZ);
                    }

                    GameObject block = selectedPrefab.Reuse(startPosition, Quaternion.identity);
                    block.transform.SetParent(transform);

                    spawnedBlocks.Add(block);

                    if (useSpawnMoveAnimation)
                    {
                        PlaySpawnMoveAnimation(block, targetPosition, z);
                    }
                }
            }
        }
    }

    [ContextMenu("Clear Spawned Blocks")]
    public void ClearSpawnedBlocks()
    {
        for (int i = spawnedBlocks.Count - 1; i >= 0; i--)
        {
            if (spawnedBlocks[i] != null)
            {
                spawnedBlocks[i].transform.DOKill();
                spawnedBlocks[i].Release();
            }
        }

        spawnedBlocks.Clear();
    }

    private void PlaySpawnMoveAnimation(GameObject block, Vector3 targetPosition, int zIndex)
    {
        block.transform.DOKill();

        float delay = 0f;

        if (useStaggerDelay)
        {
            delay = zIndex * staggerDelayPerZLayer;
        }

        block.transform
            .DOMove(targetPosition, spawnMoveDuration)
            .SetDelay(delay)
            .SetEase(spawnMoveEase);
    }

    private Vector3 GetCenteredPosition(int x, int y, int z)
    {
        float xOffset = GetCenteredAxisOffset(x, gridSize.x, spacing.x, 0f);
        float yOffset = GetCenteredAxisOffset(y, gridSize.y, spacing.y, 0f);
        float zOffset = GetCenteredAxisOffset(z, gridSize.z, spacing.z, 0f);

        return centerPosition + new Vector3(xOffset, yOffset, zOffset);
    }

    private float GetCenteredAxisOffset(int index, int count, float axisSpacing, float centerGap)
    {
        float offset = (index - (count - 1) * 0.5f) * axisSpacing;

        if (centerGap > 0f)
        {
            if (offset < 0f)
                offset -= centerGap * 0.5f;
            else if (offset > 0f)
                offset += centerGap * 0.5f;
        }

        return offset;
    }

    private GameObject GetRandomBlockPrefab()
    {
        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (GameObject prefab in blockPrefabs)
        {
            if (prefab != null)
                validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
            return null;

        int randomIndex = Random.Range(0, validPrefabs.Count);
        return validPrefabs[randomIndex];
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            centerPosition,
            new Vector3(
                (gridSize.x - 1) * spacing.x,
                (gridSize.y - 1) * spacing.y,
                (gridSize.z - 1) * spacing.z
            )
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            centerPosition + new Vector3(0f, 0f, spawnForwardOffsetZ),
            new Vector3(
                (gridSize.x - 1) * spacing.x,
                (gridSize.y - 1) * spacing.y,
                (gridSize.z - 1) * spacing.z
            )
        );
    }
#endif
}