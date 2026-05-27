using System.Collections;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;
using UnityEngine.UIElements;

public class BallSpawner : MonoBehaviour
{

    public static BallSpawner Instance { get; private set; }
    [SerializeField] private GameObject ballPrefab;

    [SerializeField] private Vector3 spawnPos;

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

        Reuse(spawnPos, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Reuse(Vector3 position, Quaternion rotation)
    {
        return ballPrefab.Reuse(position, rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // 如果 spawnPos 是世界座標
        Gizmos.DrawSphere(spawnPos, 0.2f);

    }

}
