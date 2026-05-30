using System;
using UnityEngine;

[DefaultExecutionOrder(-300)]
public class LevelStatus : MonoBehaviour
{
    public static LevelStatus Instance { get; private set; }

    public enum LevelState
    {
        None,
        Loading,
        Ready,
        Playing,
        Paused,
        Cleared,
        Failed
    }

    public LevelState CurrentState { get; private set; } = LevelState.None;

    public event Action<LevelState> OnStateChanged;

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

    private void Update()
    {
        
        if(CurrentState == LevelState.Playing)
        {
            if (LevelCounter.Instance.CurBallCount == 0)
                SetState(LevelState.Failed);
            else if (LevelCounter.Instance.CurBrickCount == 0)
                SetState(LevelState.Cleared);
        }

    }

    public void SetState(LevelState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        Debug.Log($"CurrentState : {CurrentState}");
        OnStateChanged?.Invoke(CurrentState);

    }

    public void SetLoadingState()
    {
        SetState(LevelState.Loading);

    }

    public bool IsState(LevelState state)
    {
        return CurrentState == state;
    }

    public void NotifyCurrentState(Action<LevelState> callback)
    {
        callback?.Invoke(CurrentState);
    }
}