using UnityEngine;
// ... (이전과 동일한 내용)
public enum ItemType { Equipment, Consumable, Etc }
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;
    public bool isStackable = true;
    public int quantity = 1;
}