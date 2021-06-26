using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad
{
    public static void Save(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, float money)
    {
        PlayerPrefs.SetString("IdleSave", d1 + "|" + d2 + "|" + d3 + "|" + d4 + "|" + d5 + "|" + d6 + "|" + d7 + "|" + d8 + "|" + money);
        Debug.Log("Game Saved!");
    }

    public static string Load()
    {
        string data = PlayerPrefs.GetString("IdleSave");
        Debug.Log("Game Loaded!");

        return data;
    }
}
