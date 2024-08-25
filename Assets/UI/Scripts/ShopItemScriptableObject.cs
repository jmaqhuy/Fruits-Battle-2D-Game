using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Shop Item", order = 1)]
public class ShopItemScriptableObject : ScriptableObject
{
    public string title;
    public Sprite image;
    public int basePrice;
}
