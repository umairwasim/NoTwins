using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public event Action<int> OnUpdateTurns;
    public event Action<int> OnUpdateScore;

    [SerializeField] private CardDataSO cardDataSO;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    //[SerializeField] private List<Sprite> allSprites = new();

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
    }

    public void Initialize()
    {
        gridLayoutGroup.constraintCount = cardDataSO.rows;
        gridLayoutGroup.enabled = true;
        contentSizeFitter.enabled = true;
        ResetData();
        SetupCards();
    }

    #region Setup and Hide Cards
    public void SetupCardsData(float rows, float cols)
    {
        cardDataSO.rows = (int)rows;
        cardDataSO.columns = (int)cols;
        //gridLayoutGroup.constraintCount = cardDataSO.rows;
        //gridLayoutGroup.enabled = true;
        //contentSizeFitter.enabled = true;
        //SetupCards();
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

    private void SetupCards()
    {
        //selectedCard = null;
        //matchCount = 0;
        int rand;
        int totalCardsCount = cardDataSO.rows * cardDataSO.columns;
        totalCardsCount -= (totalCardsCount % 2);
        Sprite[] allShuffledSps = new Sprite[totalCardsCount];

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
            OnUpdateTurns?.Invoke(++turns);
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
        CalculateScore();
        selectedCard = null;
        animationInProgress = false;
        matchCount += 2;
        Debug.Log("allCards  " + allCards.Count + " matchCount " + matchCount);
        if (allCards.Count == matchCount)
        {
            GameManager.Instance.GameWon();
        }
    }

    private void CalculateScore()
    {
        scorePerMatch++;
        finalScore = scorePerMatch * comboMultiplier;
        OnUpdateScore?.Invoke(finalScore);
    }
}
