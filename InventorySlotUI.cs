
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public Item item;
    public uint amount;
    public Button button;

    public void AddItem(Item newItem,uint cant)
    {
        item = newItem;
        amount = cant;
        icon.sprite = item.sprite;
        icon.enabled=true;
        icon.preserveAspect = true;
        button.enabled = true;
        //button.onClick.AddListener(delegate() { ClearSlot(); });
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        button.enabled = false;
    }

    public void Use()
    {
        item.Use();
        //ClearSlot();
    }
}
