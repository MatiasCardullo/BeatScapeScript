
using UnityEngine;

[CreateAssetMenu(fileName = "New Cartridge", menuName = "Items/Cartridge")]
public class Cartridge : Item
{
    public new string name;
    public string album;
    public string[] artists;
    //public string audioLink;
    public AudioClip audio;
    public int manaFluxUsage;
    public float speed=1, timeDuration;

    public void Start()
    {
        type = ItemType.Cartridge;
        timeDuration = audio.length;
    }

    public override void Use()
    {
        base.Use();
    }
}
