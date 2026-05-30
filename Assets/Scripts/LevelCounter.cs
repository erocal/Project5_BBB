using UnityEngine;

public class LevelCounter : MonoBehaviour
{
    public static LevelCounter Instance { get; private set; }

    [SerializeField] private int curBrickCount = 0;
    [SerializeField] private int curBallCount = 0;

    public int CurBrickCount => curBrickCount;
    public int CurBallCount => curBallCount;

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

    public void AddBall()
    {
        curBallCount++;
    }

    public void RemoveBrick()
    {
        curBrickCount--;

        if (curBrickCount < 0)
            curBrickCount = 0;
    }

    public void RemoveBall()
    {
        curBallCount--;

        if (curBallCount < 0)
            curBallCount = 0;
    }

    public void ResetBrickCount()
    {
        curBrickCount = 0;
    }

    public void ResetBallCount()
    {
        curBallCount = 0;
    }

}