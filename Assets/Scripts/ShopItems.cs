using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public string itemDescription;
    public int itemPrice;
}