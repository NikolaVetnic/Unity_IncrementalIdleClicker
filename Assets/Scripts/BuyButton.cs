using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    [HideInInspector] public int id;

    // click function on button
    public void BuyAnItem()
    {
        // buy the item
        GameManager.instance.BuyItem(id);
    }
}
