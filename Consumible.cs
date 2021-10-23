
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumible", menuName = "Items/Consumible")]
public class Consumible : Item
{
    public uint restoreHealthValue;
    public void Start()
    {
        type = ItemType.Consumible;
    }
}
