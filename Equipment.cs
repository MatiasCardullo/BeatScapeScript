
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Items/Equipment")]
public class Equipment : Item
{
    public float atkBonus;
    public float defBonus;
    public float hpBonus;
    public void Start()
    {
        type = ItemType.Equipment;
    }
}
