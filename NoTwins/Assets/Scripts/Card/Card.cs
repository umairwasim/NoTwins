using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Card : MonoBehaviour
{
    public bool isFaceUp = false;
    public bool isMatched = false;

    [SerializeField] private Sprite faceDownSprite;
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;

    private Action<Card> OnCardClick;
    public Sprite faceUpSprite;

    public Sprite GetFaceUpSprite() => faceUpSprite;

    public void Initialize(Sprite faceUpSp, Action<Card> OnClick, bool isActive = false)
    {
        faceUpSprite = faceUpSp;
        SetCardFaceUp();
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnButtonPressed);

        OnCardClick = OnClick;

        if (!isActive)
            gameObject.SetActive(true);
        else
            StartCoroutine(HideMatchedCardRoutine());
        // SetCardMatched();
    }


    public bool HasSameSprite(Card card)
    {
        if (cardImage.sprite == card.cardImage.sprite)
            return true;
        return false;
    }

    private void OnButtonPressed()
    {
        OnCardClick.Invoke(this);
    }

    public void FlipCard()
    {
        if (isFaceUp)
            SetCardFaceDown();
        else
            SetCardFaceUp();
    }

    public void SetCardFaceUp()
    {
        isFaceUp = true;
        cardImage.sprite = faceUpSprite;
        cardImage.transform.DOScale(1.2f, 0.25f).SetEase(Ease.Flash);
    }

    public void SetCardFaceDown()
    {
        isFaceUp = false;
        cardImage.sprite = faceDownSprite;
        cardImage.transform.DOScale(1f, 0.25f);

    }

    IEnumerator HideMatchedCardRoutine()
    {
        yield return new WaitForSeconds(2f);
        SetCardMatched();
    }

    public void SetCardMatched()
    {
        isMatched = true;
        cardImage.DOFade(0f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}

