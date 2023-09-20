using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("Card")]
    [SerializeField] private CardDataSO cardDataSO;
    [SerializeField] private Card cardPrefab;

    [Header("Grid")]
    [SerializeField] private Transform parent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private ContentSizeFitter contentSizeFitter;

    private List<Card> allCards = new();
    private Card selectedCard = null;

    private bool animationInProgress = false;
    private int turns = 0;
    private int scorePerMatch = 1;
    private int finalScore = 0;
    private int comboMultiplier = 1;
    private int matchCount = 0;

    private void Awake()
    {
        Instance = this;

        //Initialize Save System
        SaveSystem.Initialize();
    }

    public void Initialize()
    {
        gridLayoutGroup.constraintCount = cardDataSO.rows;
        gridLayoutGroup.enabled = true;
        contentSizeFitter.enabled = true;
        ResetData();
    }

    #region Save/Load Data
    public void SaveData()
    {
        SavePlayerData saveObject = new();

        saveObject.turns = turns;
        saveObject.score = finalScore;
        saveObject.comboMultipler = comboMultiplier;
        saveObject.matchCount = matchCount;
        saveObject.rows = cardDataSO.rows;
        saveObject.columns = cardDataSO.columns;

        for (int i = 0; i < allCards.Count; i++)
        {
            saveObject.cardPositions.Add(allCards[i].faceUpSprite);
            saveObject.isMatched.Add(allCards[i].isMatched);
        }

        string dataToSave = JsonUtility.ToJson(saveObject);
        SaveSystem.SaveData(dataToSave);
    }

    //Load Saved Data from SavePlayerData
    public void LoadData()
    {
        string saveString = SaveSystem.LoadData();
        if (saveString != null)
        {
            SavePlayerData saveObject = JsonUtility.FromJson<SavePlayerData>(saveString);
            Card card;

            turns = saveObject.turns;
            finalScore = saveObject.score;
            comboMultiplier = saveObject.comboMultipler;
            matchCount = saveObject.matchCount;
            cardDataSO.rows = saveObject.rows;
            cardDataSO.columns = saveObject.columns;

            //Update UI
            GameManager.Instance.OnUpdateGameUI(finalScore, turns);

            //Spawn and initialize with saved card data
            for (int i = 0; i < saveObject.cardPositions.Count; i++)
            {
                card = Instantiate(cardPrefab, parent);
                allCards.Add(card);
                card.Initialize(saveObject.cardPositions[i], OnCardClicked, saveObject.isMatched[i]);
            }
            StartCoroutine(HideCardsRoutine());
        }
        else
        {
            Debug.Log("No save string found");
        }
    }
    private void ResetData()
    {
        selectedCard = null;
        matchCount = 0;
        turns = 0;
        scorePerMatch = 1;
        finalScore = 0;
        comboMultiplier = 1;
        allCards.Clear();
    }
    #endregion

    #region Setup, Show/Hide Cards
    public void SetupCardsData(float rows, float cols)
    {
        cardDataSO.rows = (int)rows;
        cardDataSO.columns = (int)cols;
    }

    public void SetupCards()
    {
        int rand;
        int totalCardsCount = cardDataSO.rows * cardDataSO.columns;
        totalCardsCount -= (totalCardsCount % 2);
        Sprite[] allShuffledSps = new Sprite[totalCardsCount];

        #region Randon Sprites placement
        for (int i = 0; i < totalCardsCount / 2; i++)
        {
            Sprite randSprite = cardDataSO.allSprites[Random.Range(0, cardDataSO.allSprites.Count)];
            int placements = 2;

            while (placements > 0)
            {
                rand = Random.Range(0, allShuffledSps.Length);
                if (allShuffledSps[rand] == null)
                {
                    placements--;
                    allShuffledSps[rand] = randSprite;
                }
            }
        }
        #endregion

        #region Instantiate Cards
        for (int i = 0; i < allShuffledSps.Length; i++)
        {
            Card card;
            if (allCards.Count >= i)
            {
                card = Instantiate(cardPrefab, parent);
                allCards.Add(card);
            }
            else
            {
                if (allCards[i] == null)
                {
                    card = Instantiate(cardPrefab, parent);
                    allCards.Add(card);
                }
                else
                    card = allCards[i];
            }

            card.Initialize(allShuffledSps[i], OnCardClicked);
        }
        #endregion

        StartCoroutine(HideCardsRoutine());
    }

    private IEnumerator HideCardsRoutine()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].SetCardFaceDown();
        }

        gridLayoutGroup.enabled = false;
        contentSizeFitter.enabled = false;
    }
    #endregion

    #region Card Selection
    private void OnCardClicked(Card card)
    {
        if (animationInProgress)
            return;

        card.FlipCard();

        if (!card.isFaceUp || card == selectedCard)
            return;

        SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClickSound);

        if (selectedCard == null)
        {
            selectedCard = card;
        }
        else
        {
            animationInProgress = true;
            if (card.HasSameSprite(selectedCard))
            {
                //Show both matching cards and disable them
                card.SetCardFaceUp();
                StartCoroutine(DisableMatchingCards(selectedCard, card));
            }
            else
            {
                //Hide non-matching cards
                StartCoroutine(DisableNonMatchingCards(selectedCard, card));
            }

            turns++;
            GameManager.Instance.OnUpdateGameUI(finalScore, turns);
            //OnUpdateTurns?.Invoke(++turns);
        }
    }

    private IEnumerator DisableNonMatchingCards(Card firstCard, Card secondCard)
    {
        comboMultiplier = 1;
        SoundManager.Instance.PlaySound(SoundManager.Instance.mismatchedSound);

        yield return new WaitForSeconds(1f);
        firstCard.SetCardFaceDown();
        secondCard.SetCardFaceDown();
        selectedCard = null;
        animationInProgress = false;
    }

    private IEnumerator DisableMatchingCards(Card firstCard, Card secondCard)
    {
        comboMultiplier++;
        SoundManager.Instance.PlaySound(SoundManager.Instance.matchedSound);

        yield return new WaitForSeconds(1f);
        firstCard.SetCardMatched();
        secondCard.SetCardMatched();
        CalculateFinalScore();

        selectedCard = null;
        animationInProgress = false;
        matchCount += 2;

        if (allCards.Count == matchCount)
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.GameWon();
        }
    }
    #endregion

    private void CalculateFinalScore()
    {
        scorePerMatch++;
        finalScore = scorePerMatch * comboMultiplier;
        GameManager.Instance.OnUpdateGameUI(finalScore, turns);
    }
}
