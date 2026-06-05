using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button replayButton;
    [SerializeField] private TMP_InputField inputFieldName;
    [SerializeField] private Rank rank;

    private bool hasLoggedResult;

    public string InputName => inputFieldName.text;

    private void OnEnable()
    {
        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += UIManagerHandleLevelStateChanged;
    }

    private void Update()
    {
        if (LevelStatus.Instance == null)
            return;

        replayButton.interactable = LevelStatus.Instance.IsState(LevelStatus.LevelState.Failed);
    }

    private void UIManagerHandleLevelStateChanged(LevelStatus.LevelState state)
    {
        switch (state)
        {
            case LevelStatus.LevelState.Loading:
                hasLoggedResult = false;
                break;

            case LevelStatus.LevelState.Ready:
                break;

            case LevelStatus.LevelState.Playing:
                break;

            case LevelStatus.LevelState.Cleared:
                break;

            case LevelStatus.LevelState.Failed:
                SaveAndPrintResult();
                break;
        }
    }

    private void SaveAndPrintResult()
    {
        if (hasLoggedResult)
            return;

        hasLoggedResult = true;

        int finalScore = GetFinalScore();

        rank.UpdateRank(InputName, finalScore);
        rank.PrintRank();
    }

    private int GetFinalScore()
    {

        return Random.Range(0, 20000);
    }

    private void OnDisable()
    {
        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= UIManagerHandleLevelStateChanged;
    }
}