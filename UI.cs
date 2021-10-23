
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public byte index;
    public bool[] showUI = new bool[] { false, false, false };
    public GameObject levelText, cameraMap, mapUI;

    public Inventory inventory;
    public Transform itemsParent;
    public InventorySlotUI[] slots;
    public GameObject inventoryUI;

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlotUI>();
    }

    void Update()
    {
        InputKey();
        if (showUI[0])
            levelText.GetComponent<Text>().text = "LEVEL: " + User.level + " ENEMIES: " + GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Container.Count)
                slots[i].AddItem(inventory.Container[i].item, inventory.Container[i].amount);
            else
                slots[i].ClearSlot();
        }
    }

    void InputKey()
    {
        if (Input.GetKeyUp("m"))
        {
            index = 0;
            showUI[0] = !showUI[0];
        }
        else if (Input.GetKeyUp("i"))
        {
            index = 1;
            showUI[1] = !showUI[1];
        }
        else if (Input.GetKeyUp("u"))
        {
            index = 2;
        }
        for (int i = 0; i < showUI.Length; i++)
        {
            if (i != index)
            {
                showUI[i] = false;
            }
        }
        cameraMap.SetActive(showUI[0]);
        mapUI.SetActive(showUI[0]);
        inventoryUI.SetActive(showUI[1]);
    }
}
