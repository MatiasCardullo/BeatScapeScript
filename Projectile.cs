
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1.2f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        string name = col.gameObject.name;
        if (name != "Fire1(Clone)" && name != "Fire2(Clone)" && !col.CompareTag("Enemy") && !col.CompareTag("Coin")
             && !col.CompareTag("Crate") && !col.CompareTag("Player") && name != "RoomCollider" && name != "CameraTarget")
        {
            Destroy(gameObject);
        }
    }
}
