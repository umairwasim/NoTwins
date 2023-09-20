using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/Card Data")]
public class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    public Sprite backSprite;
}
