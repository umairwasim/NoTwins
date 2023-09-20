using System;
using UnityEngine;

public enum GameState
{
    Main,
    Gameplay,
    Won,
    LoadGame,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action<int, int> OnUpdateUI;

    [Header("Manager Reference")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CardManager cardManager;

    private GameState gameState = GameState.Main;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeGameState(GameState.Main);
    }

    private void ChangeGameState(GameState currentState)
    {
        gameState = currentState;

        switch (gameState)
        {
            case GameState.Main:
                OnMainMenuSelected();
                break;
            case GameState.Gameplay:
                OnGameplayMenuSelected();
                break;
            case GameState.Won:
                OnWonMenuSelected();
                break;
            case GameState.LoadGame:
                OnResumeGameplayMenuSelected();
                break;
        }
    }

    #region On Menu Selection
    private void OnMainMenuSelected()
    {
        uiManager.ActivateMainMenu();
    }

    private void OnGameplayMenuSelected()
    {
        uiManager.ActivateGamePlayMenu();
        cardManager.Initialize();
        cardManager.SetupCards();
    }

    private void OnResumeGameplayMenuSelected()
    {
        uiManager.ActivateGamePlayMenu();
        cardManager.Initialize();
        cardManager.LoadData();
    }

    private void OnWonMenuSelected()
    {
        uiManager.ActivateWonMenu();
    }
    #endregion

    #region Game State Functions
    public void PlayGame()
    {
        ChangeGameState(GameState.Gameplay);
        SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClickSound);
    }

    public void LoadGame()
    {
        ChangeGameState(GameState.LoadGame);
        SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClickSound);
    }


    public void GameWon()
    {
        ChangeGameState(GameState.Won);
    }
    #endregion

    public void OnUpdateGameUI(int score = 0, int turns = 0)
    {
        OnUpdateUI?.Invoke(score, turns);
    }

    public void OnSetupCardsData(float rows, float cols)
    {
        cardManager.SetupCardsData(rows, cols);
    }

    #region Save / Load Data
    public void SaveData()
    {
        cardManager.SaveData();
    }

    public void LoadData()
    {
        cardManager.LoadData();
    }
    #endregion
}