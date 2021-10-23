
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed;
    private Transform target;
    private Vector2 shootTarget;
    Rigidbody2D projectileRB;
    
    void Start()
    {
        projectileRB = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        shootTarget = (target.transform.position-transform.position).normalized*speed;
        projectileRB.velocity = new Vector2(shootTarget.x, shootTarget.y);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        string name = col.gameObject.name;
        if (name != "Fire1(Clone)" && name != "Fire2(Clone)" && name != "RoomCollider" && name != "CameraTarget" && !col.CompareTag("Coin") && !col.CompareTag("Crate") && !col.CompareTag("Enemy") && !col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
