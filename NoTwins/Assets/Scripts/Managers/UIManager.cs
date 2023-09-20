using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum MenuState
{
    Main,
    Game,
    Won,
}

public class UIManager : MonoBehaviour
{

    public MenuState menuState = MenuState.Main;

    [Header("Menus")]
    [SerializeField] private GameObject[] menus; // 0 = main, 1 = game, 2 = won,

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button nextButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnsText;

    [Header("Sliders")]
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private Slider columnsSlider;
    [SerializeField] private TMP_Text rowsCountText;
    [SerializeField] private TMP_Text columnsCountText;

    private int score;
    private int turns;

    private void Start()
    {
        // ActivateMenu(MenuState.Main);
        CardManager.Instance.OnUpdateScore += CardManager_OnUpdateScore;
        CardManager.Instance.OnUpdateTurns += CardManager_OnUpdateTurns;
        playButton.onClick.AddListener(PlayButtonPressed);
        nextButton.onClick.AddListener(NextButtonPressed);
        rowsSlider.onValueChanged.AddListener(delegate { OnRowsSliderValueChanged(); });
        columnsSlider.onValueChanged.AddListener(delegate { OnColumnsSliderValueChanged(); });
    }

    private void OnDestroy()
    {
        CardManager.Instance.OnUpdateScore -= CardManager_OnUpdateScore;
        CardManager.Instance.OnUpdateTurns -= CardManager_OnUpdateTurns;
        playButton.onClick.RemoveListener(PlayButtonPressed);
        nextButton.onClick.RemoveListener(NextButtonPressed);
        rowsSlider.onValueChanged.RemoveListener(delegate { OnRowsSliderValueChanged(); });
        columnsSlider.onValueChanged.RemoveListener(delegate { OnColumnsSliderValueChanged(); });
    }

    private void OnRowsSliderValueChanged()
    {
        rowsCountText.text = rowsSlider.value.ToString();
    }

    private void OnColumnsSliderValueChanged()
    {
        columnsCountText.text = columnsSlider.value.ToString();
    }

    private void CardManager_OnUpdateTurns(int obj)
    {
        turns = obj;
        turnsText.text = "TURNS: " + turns;
    }

    private void CardManager_OnUpdateScore(int obj)
    {
        score = obj;
        scoreText.text = "SCORE: " + score;
    }

    public void ActivateMenu(MenuState menuToActivate)
    {
        for (int i = 0; i < menus.Length; i++)
            menus[i].SetActive(false);

        menus[(int)menuToActivate].SetActive(true);
    }

    public void ActivateMainMenu()
    {
        menuState = MenuState.Main;
        ActivateMenu(menuState);
    }

    public void ActivateGamePlayMenu()
    {
        menuState = MenuState.Game;
        ActivateMenu(menuState);
    }

    public void ActivateWonMenu()
    {
        menuState = MenuState.Won;
        ActivateMenu(menuState);
    }

    public void PlayButtonPressed()
    {
        CardManager.Instance.SetupCardsData(rowsSlider.value, columnsSlider.value);
        SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClickSound);
        GameManager.Instance.PlayGame();
    }

    public void NextButtonPressed()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClickSound);
        GameManager.Instance.PlayGame();
    }

}
