using System;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public bool isFaceUp = false;

    [SerializeField] private Sprite faceDownSprite;
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;

    private Action<Card> OnCardClick;
    private Sprite faceUpSprite;

    public void Initialize(Sprite faceUpSp, Action<Card> OnClick)
    {
        faceUpSprite = faceUpSp;
        SetCardFaceUp();
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnButtonPressed);

        OnCardClick = OnClick;
        gameObject.SetActive(true);
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
    }

    public void SetCardFaceDown()
    {
        isFaceUp = false;
        cardImage.sprite = faceDownSprite;
    }

    public void SetCardMatched()
    {
        gameObject.SetActive(false);
    }
}

