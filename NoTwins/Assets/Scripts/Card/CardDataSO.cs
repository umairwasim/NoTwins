using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsData", menuName = "CardData/Data")]
public class CardDataSO : ScriptableObject
{
    [Header("Grid")]
    [Range(3, 6)] public int rows;
    [Range(3, 6)] public int columns;

    [Header("Sprites")]
    public List<Sprite> allSprites = new();
}
