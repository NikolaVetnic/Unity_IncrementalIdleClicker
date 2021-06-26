using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [System.Serializable]
    public class AnyDrink
    {
        [HideInInspector] public int drinkAmount;       // amount
        public Drink drink;                             // scriptable object
        public bool unlocked;                           // has been unlocked
        [HideInInspector] public bool instanced;        // has been instantiated
        [HideInInspector] public ItemHolder holder;
    }

    public List<AnyDrink> drinkList = new List<AnyDrink>();

    // MONEY
    [Header("MONEY")]
    public float money;
    public Text totalMoneyText;
    public Text totalGPSText;

    // USER INTERFACE
    [Header("USER INTERFACE")]
    public GameObject itemHolderUI;
    public Transform grid;

    // SAVING
    public GameObject saveText;

    // called before Start();
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        saveText.SetActive(false);

        if (PlayerPrefs.HasKey("IdleSave"))
        {
            LoadTheGame();
        }
        else
        {
            FillList();
        }

        UpdateMoneyUI();
        CalculateGPS();
        StartCoroutine(Tick());

        AutoSave();
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            
            foreach(AnyDrink d in drinkList)
            {
                if (d.drinkAmount > 0)
                {
                    money += d.drink.CalculateIncome(d.drinkAmount);
                    money = (float) Mathf.Round(money * 100) / 100;
                    
                    UpdateMoneyUI();
                }
            }
        }
    }

    void FillList()
    {
        for (int i = 0; i < drinkList.Count; i++)
        {
            if (drinkList[i].unlocked)
            {
                if (drinkList[i].drinkAmount > 0 || drinkList[i].instanced)
                {
                    // skip this item
                    continue;
                }

                GameObject itemHolder = Instantiate(itemHolderUI, grid, false) as GameObject;
                drinkList[i].holder = itemHolder.GetComponent<ItemHolder>();
                
                // is it discovered already
                if (drinkList[i].drinkAmount > 0)
                {
                    drinkList[i].holder.itemImage.sprite = drinkList[i].drink.drinkImage;
                    drinkList[i].holder.itemNameText.text = drinkList[i].drink.drinkName;
                    drinkList[i].holder.amountText.text = "Amount : " + drinkList[i].drinkAmount.ToString("N0");
                    drinkList[i].holder.gpsText.text = "GPS : " + drinkList[i].drink.CalculateIncome(drinkList[i].drinkAmount).ToString("N2");
                    drinkList[i].holder.costText.text = "Cost : " + drinkList[i].drink.CalculateCost(drinkList[i].drinkAmount).ToString("N2");
                }
                else 
                {
                    drinkList[i].holder.itemImage.sprite = drinkList[i].drink.unknownDrinkImage;
                    drinkList[i].holder.itemNameText.text = "?????";
                    drinkList[i].holder.amountText.text = "Amount : " + drinkList[i].drinkAmount.ToString("N0");
                    drinkList[i].holder.gpsText.text = "GPS : " + drinkList[i].drink.CalculateIncome(drinkList[i].drinkAmount).ToString("N2");
                    drinkList[i].holder.costText.text = "Cost : " + drinkList[i].drink.CalculateCost(drinkList[i].drinkAmount).ToString("N2");
                }

                drinkList[i].holder.buyButton.id = i;
                drinkList[i].instanced = true;
            }
        }
    }

    public void BuyItem(int id)
    {
        if (money < drinkList[id].drink.CalculateCost(drinkList[id].drinkAmount))
        {
            Debug.Log("NOT ENOUGH MONEY");
            return;
        }

        // decrease money
        money -= drinkList[id].drink.CalculateCost(drinkList[id].drinkAmount);
        
        // update UI in holder
        if (drinkList[id].drinkAmount < 1)
        {
            drinkList[id].holder.itemImage.sprite = drinkList[id].drink.drinkImage;
            drinkList[id].holder.itemNameText.text = drinkList[id].drink.drinkName;
        }
        
        drinkList[id].drinkAmount++;
        drinkList[id].holder.amountText.text = "Amount : " + drinkList[id].drinkAmount.ToString("N0");
        drinkList[id].holder.gpsText.text = "GPS : " + drinkList[id].drink.CalculateIncome(drinkList[id].drinkAmount).ToString("N2");
        drinkList[id].holder.costText.text = "Cost : " + drinkList[id].drink.CalculateCost(drinkList[id].drinkAmount).ToString("N2");

        // unlock the next drink
        if (id < drinkList.Count - 1 && drinkList[id].drinkAmount > 0)
        {
            drinkList[id+1].unlocked = true;
            FillList();
        }

        // update GPS & money UI
        CalculateGPS();
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;

        // update money UI
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        totalMoneyText.text = "Total Money: " + money.ToString("N2");
    }

    void CalculateGPS()
    {
        float allGPS = 0;

        foreach(AnyDrink d in drinkList)
        {
            if (d.drinkAmount > 0)
            {
                allGPS += d.drink.CalculateIncome(d.drinkAmount);
                totalGPSText.text = "Total GPS : " + allGPS.ToString("N2");
            }
        }

        if (allGPS == 0)
        {
            totalGPSText.text = "Total GPS : " + allGPS.ToString("N2");
        }
    }

    void SaveTheGame()
    {
        SaveLoad.Save(
            drinkList[0].drinkAmount, drinkList[1].drinkAmount, drinkList[2].drinkAmount, drinkList[3].drinkAmount, 
            drinkList[4].drinkAmount, drinkList[5].drinkAmount, drinkList[6].drinkAmount, drinkList[7].drinkAmount, money);
    }

    void AutoSave()
    {
        SaveTheGame();
        saveText.SetActive(true);

        Invoke("AutoSave", 60f);
    }

    void LoadTheGame()
    {
        if (PlayerPrefs.HasKey("IdleSave"))
        {
            string data = SaveLoad.Load();
            string[] stringList = data.Split("|"[0]);

            // we do not process money, hence Length - 1
            for (int i = 0; i < stringList.Length - 1; i++)
            {
                int temp = int.Parse(stringList[i]);
                drinkList[i].drinkAmount = temp;

                if (temp > 0)
                {
                    if (i + 1 < drinkList.Count) {
                        drinkList[i+1].unlocked = true;
                    } 

                    // fill the list
                    FillSingleItem(i);
                }
            }

            money = float.Parse(stringList[8]);

            FillList();

            UpdateMoneyUI();
            CalculateGPS();
        }
    }

    void FillSingleItem(int id)
    {
        if (drinkList[id].unlocked)
        {
            GameObject itemHolder = Instantiate(itemHolderUI, grid, false) as GameObject;
            drinkList[id].holder = itemHolder.GetComponent<ItemHolder>();
                
            if (drinkList[id].drinkAmount > 0)
            {
                drinkList[id].holder.itemImage.sprite = drinkList[id].drink.drinkImage;
                drinkList[id].holder.itemNameText.text = drinkList[id].drink.drinkName;
                drinkList[id].holder.amountText.text = "Amount : " + drinkList[id].drinkAmount.ToString("N0");
                drinkList[id].holder.gpsText.text = "GPS : " + drinkList[id].drink.CalculateIncome(drinkList[id].drinkAmount).ToString("N2");
                drinkList[id].holder.costText.text = "Cost : " + drinkList[id].drink.CalculateCost(drinkList[id].drinkAmount).ToString("N2");
            }
            else 
            {
                drinkList[id].holder.itemImage.sprite = drinkList[id].drink.unknownDrinkImage;
                drinkList[id].holder.itemNameText.text = "?????";
                drinkList[id].holder.amountText.text = "Amount : " + drinkList[id].drinkAmount.ToString("N0");
                drinkList[id].holder.gpsText.text = "GPS : " + drinkList[id].drink.CalculateIncome(drinkList[id].drinkAmount).ToString("N2");
                drinkList[id].holder.costText.text = "Cost : " + drinkList[id].drink.CalculateCost(drinkList[id].drinkAmount).ToString("N2");
            }

            drinkList[id].holder.buyButton.id = id;
            drinkList[id].instanced = true;
        }
    }
}
