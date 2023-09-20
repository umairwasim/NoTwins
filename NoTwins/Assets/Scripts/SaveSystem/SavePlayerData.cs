using System.Collections.Generic;
using UnityEngine;

public class SavePlayerData
{
    public int turns;
    public int score;
    public int comboMultipler;
    public int matchCount;
    public int rows;
    public int columns;
    public List<Sprite> cardPositions = new();
    public List<bool> isMatched = new();

}
