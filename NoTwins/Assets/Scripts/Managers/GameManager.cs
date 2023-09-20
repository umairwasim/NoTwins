using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Main,
    Gameplay,
    Won,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    public void GameWon()
    {
        ChangeGameState(GameState.Won);
    }
    #endregion

    //public void RestartGame()
    //{
    //    ChangeGameState(GameState.Won);
    //    uiManager.PlayButtonPressed();
    //}
}