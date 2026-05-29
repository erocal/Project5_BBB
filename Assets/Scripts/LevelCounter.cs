using UnityEngine;

public class LevelCounter : MonoBehaviour
{
    public static LevelCounter Instance { get; private set; }

    [SerializeField]
    private int curBrickCount = 0;

    public int CurBrickCount => curBrickCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddBrick()
    {
        curBrickCount++;
    }

    public void RemoveBrick()
    {
        curBrickCount--;

        if (curBrickCount < 0)
            curBrickCount = 0;
    }

    public void ResetBrickCount()
    {
        curBrickCount = 0;
    }
}