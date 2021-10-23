using UnityEngine;

public class Enemy : ChildsRoom
{
    private bool inScene;
    public static float speed;
    private float followDistance, stopDistance, retreatDistance, distance, nextFireTime, fireRate;
    private Vector2 fireVector,targetVector;
    public GameObject firePrefab;
    public GameObject[] coinPrefabs;
    private Transform target;
    public Animator anim;

    void Start()
    {
        life = 20; fireRate = 0.5f;
        speed = 2; followDistance = 3; stopDistance = 1.5f; retreatDistance = 1;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        base.Base();
        base.OnFire(0.1f,0.1f);
        distance = Vector2.Distance(transform.position, target.position);
        if (distance < followDistance)
        {
            if (distance > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else if (distance < retreatDistance)
            {
                // new Vector2 (target.position.x * Random.Range(target.position.x/ transform.position.x* - 10, target.position.x / transform.position.x * 10),target.position.y * Random.Range(target.position.y / transform.position.y * -10, target.position.y / transform.position.y * 10))
                transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * retreatDistance / distance * Time.deltaTime);
            }
            if (inScene)
            {
                targetVector = (target.position - transform.position).normalized;
                anim.SetFloat("Horizontal", targetVector.x);
                anim.SetFloat("Vertical", targetVector.y);
                if (nextFireTime < Time.time)
                {
                    fireVector = targetVector;
                    fireVector *= 0.1f;
                    fireVector = new Vector2(transform.position.x, (transform.position.y + 0.2f)) + fireVector;
                    Instantiate(firePrefab, fireVector, transform.rotation, transform);
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
        if (life <= 0)
        {
            GameObject obj;
            for (byte i = 0; i < Random.Range(5, 10); i++)
            {
                obj = Instantiate(coinPrefabs[Random.Range(0, coinPrefabs.Length-1)],
                    new Vector2(transform.position.x+Random.Range(-0.2f,0.2f), transform.position.y + Random.Range(-0.2f, 0.2f)),
                    transform.rotation, GameObject.Find("Items lv"+User.level).transform);
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            }
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Fire1(Clone)")
        {
            life--;
            Destroy(col.gameObject);
        }
        if (col.gameObject.name == "FireBarrel")
        {
            inFire = true;
        }
        if (!inScene && col.gameObject.name == "CameraTarget")
        {
            inScene = true;
            anim.enabled = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (inScene && "CameraTarget" == col.gameObject.name)
        {
            inScene = false;
            anim.enabled = false;
        }
        if (col.gameObject.name == "FireBarrel")
        {
            inFire = false;
        }
    }
}
