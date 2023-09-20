using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnsText;
    [SerializeField] private TMP_Text rowsCountText;
    [SerializeField] private TMP_Text columnsCountText;

    [Header("Sliders")]
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private Slider columnsSlider;

    private int score;
    private int turns;

    private void Start()
    {
        GameManager.Instance.OnUpdateUI += GameManager_OnUpdateUI;

        playButton.onClick.AddListener(PlayButtonPressed);
        nextButton.onClick.AddListener(NextButtonPressed);
        saveButton.onClick.AddListener(SaveButtonPressed);
        loadButton.onClick.AddListener(LoadButtonPressed);

        rowsSlider.onValueChanged.AddListener(delegate { OnRowsSliderValueChanged(); });
        columnsSlider.onValueChanged.AddListener(delegate { OnColumnsSliderValueChanged(); });

        //only show load button if save file exists
        if (SaveSystem.FileExists())
            loadButton.gameObject.SetActive(true);
        else
            loadButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnUpdateUI -= GameManager_OnUpdateUI;

        playButton.onClick.RemoveListener(PlayButtonPressed);
        nextButton.onClick.RemoveListener(NextButtonPressed);
        saveButton.onClick.RemoveListener(SaveButtonPressed);
        loadButton.onClick.RemoveListener(LoadButtonPressed);

        rowsSlider.onValueChanged.RemoveListener(delegate { OnRowsSliderValueChanged(); });
        columnsSlider.onValueChanged.RemoveListener(delegate { OnColumnsSliderValueChanged(); });
    }

    public void ActivateMenu(MenuState menuToActivate)
    {
        for (int i = 0; i < menus.Length; i++)
            menus[i].SetActive(false);

        menus[(int)menuToActivate].SetActive(true);

        if ((int)menuToActivate != 0)
            menus[(int)menuToActivate].transform.DOPunchScale(Vector3.one * 1.5f, 0.2f);
    }

    public void ResetUI()
    {
        turnsText.text = "TURNS: " + 0;
        scoreText.text = "SCORE: " + 0;
    }

    #region UI Listeners

    private void GameManager_OnUpdateUI(int arg1, int arg2)
    {
        score = arg1;
        turns = arg2;
        scoreText.text = "SCORE: " + score;
        turnsText.text = "TURNS: " + turns;
    }

    private void OnRowsSliderValueChanged()
    {
        rowsCountText.text = rowsSlider.value.ToString();
    }

    private void OnColumnsSliderValueChanged()
    {
        columnsCountText.text = columnsSlider.value.ToString();
    }
    #endregion

    #region Activate Menus

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
        ResetUI();
    }
    #endregion

    #region Buttons 
    public void PlayButtonPressed()
    {
        GameManager.Instance.OnSetupCardsData(rowsSlider.value, columnsSlider.value);
        GameManager.Instance.PlayGame();
    }

    public void NextButtonPressed()
    {
        GameManager.Instance.PlayGame();
    }

    public void SaveButtonPressed()
    {
        GameManager.Instance.SaveData();
    }

    public void LoadButtonPressed()
    {
        GameManager.Instance.LoadGame();
    }
    #endregion
}
