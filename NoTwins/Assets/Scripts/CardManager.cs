using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [Range(3, 6), SerializeField] private int numRows = 4;
    [Range(3, 6), SerializeField] private int numCols = 4;
    [SerializeField] private List<Sprite> allSprites = new();

    private List<Card> allCards = new();
    private Card selectedCard = null;
    private bool animationInProgress = false;

    private void Start()
    {
        gridLayoutGroup.constraintCount = numRows;
        contentSizeFitter.enabled = true;
        SetupCards();
    }

    private void SetupCards()
    {
        selectedCard = null;

        int totalCardsCount = numRows * numCols;
        totalCardsCount = totalCardsCount - (totalCardsCount % 2);
        Sprite[] allShuffledSps = new Sprite[totalCardsCount];

        int rand;

        for (int i = 0; i < totalCardsCount / 2; i++)
        {
            Sprite randSprite = allSprites[Random.Range(0, allSprites.Count)];
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
            Card card = Instantiate(cardPrefab, parent);
            card.Initialize(allShuffledSps[i], OnCardClicked);
            allCards.Add(card);
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

    private void OnCardClicked(Card card)
    {
        if (animationInProgress)
            return;

        card.FlipCard();

        if (!card.isFaceUp || card == selectedCard)
            return;

        if (selectedCard == null)
        {
            selectedCard = card;
        }
        else
        {
            animationInProgress = true;
            if (card.HasSameSprite(selectedCard))
            {
                //Show both cards and disable them
                card.SetCardFaceUp();
                StartCoroutine(DisableMatchingCards(selectedCard, card));
            }
            else
            {
                StartCoroutine(DisableNonMatchingCards(selectedCard, card));
            }
        }
    }

    private IEnumerator DisableNonMatchingCards(Card firstCard, Card secondCard)
    {
        yield return new WaitForSeconds(1f);
        firstCard.SetCardFaceDown();
        secondCard.SetCardFaceDown();
        selectedCard = null;
        animationInProgress = false;
    }

    private IEnumerator DisableMatchingCards(Card firstCard, Card secondCard)
    {
        yield return new WaitForSeconds(1f);
        firstCard.SetCardMatched();
        secondCard.SetCardMatched();
        selectedCard = null;
        animationInProgress = false;
    }
}
