using System.Collections;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Button replayButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged += UIManagerHandleLevelStateChanged;

    }

    // Update is called once per frame
    void Update()
    {

        replayButton.interactable = LevelStatus.Instance.IsState(LevelStatus.LevelState.Failed);


    }

    private void UIManagerHandleLevelStateChanged(LevelStatus.LevelState state)
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

                break;
        }
    }

    private void OnDisable()
    {

        if (LevelStatus.Instance == null)
            return;

        LevelStatus.Instance.OnStateChanged -= UIManagerHandleLevelStateChanged;

    }

}
