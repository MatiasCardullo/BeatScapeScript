
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
}
