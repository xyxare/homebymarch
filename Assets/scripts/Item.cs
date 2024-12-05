[System.Serializable]

public enum ItemTypes {Consumable, Weapon, Armor}

public class Item
{
    public string itemName;
    public int price;
    public ItemTypes itemType;

    

    public Item(string name, int cost, string type)
    {
        itemName = name;
        price = cost;
        
        switch(type){
            case "consumable":
            itemType = ItemTypes.Consumable;
            break;
            case "weapon":
            itemType = ItemTypes.Weapon;
            break;
            case "armor":
            itemType = ItemTypes.Armor;
            break;
        }
    }
}
