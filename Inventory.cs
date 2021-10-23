
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(Item _item, uint _amount)
    {
        bool hasItem = false;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                hasItem = true;
                break;
            }

        }
        if (!hasItem)
        {
            Container.Add(new InventorySlot(_item, _amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public uint amount;

    public InventorySlot(Item _item, uint _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(uint value)
    {
        amount += value;
    }
}
