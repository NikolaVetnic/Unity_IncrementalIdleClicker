using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an asset, a scriptable object

[CreateAssetMenu(fileName="New Drink", menuName="Idle Game/Drinks")]
public class Drink : ScriptableObject
{
    // we use public so that we see variables in the Inspector
    public string drinkName;
    public float basePrice;
    public float multiplier = 1.15f;    // [1.07 - 1.15]
    public float baseIncome;

    public Sprite drinkImage;
    public Sprite unknownDrinkImage;

    // price = basePrice * multiplier^N, where N is the number we already have
    public float CalculateCost(int amount)
    {
        // exponential
        float newPrice = basePrice * Mathf.Pow(multiplier, amount);
        // float newPrice = basePrice * Mathf.pow(multiplier, (float) amount);
        float rounded = (float) Mathf.Round(newPrice * 100) / 100;

        return rounded;
    }

    public float CalculateIncome(int amount)
    {
        // linear
        return baseIncome * amount;
    }
}
