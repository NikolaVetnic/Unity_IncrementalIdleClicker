using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(Drink))]
public class AssetPreviewer : Editor
{
    Drink drink;
    Texture2D tex1;
    Texture2D tex2;

    public override void OnInspectorGUI()
    {
        drink = (Drink) target;

        GUILayout.BeginHorizontal();    

            tex1 = AssetPreview.GetAssetPreview(drink.drinkImage);
            GUILayout.Label(tex1);
            

            tex2 = AssetPreview.GetAssetPreview(drink.unknownDrinkImage);
            GUILayout.Label(tex2);

        GUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
