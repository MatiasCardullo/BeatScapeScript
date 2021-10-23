using UnityEngine;

public class Crate : ChildsRoom
{
    public GameObject[] coinPrefabs;
    public GameObject itemContainer;
    private Transform itemParent;

    void Start()
    {
        life = 10;
        itemParent = GameObject.Find("Items lv" + User.level).transform;
        //Items = Resources.LoadAll<Item>("Items");
    }

    void Update()
    {
        base.Base();
        base.OnFire(0.1f, 0.02f);
        if (life <= 0)
        {
            GameObject obj;
            for (byte i = 0; i < Random.Range(10, 20); i++)
            {
                obj = Instantiate(coinPrefabs[Random.Range(0, coinPrefabs.Length - 1)],
                    new Vector2(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y + Random.Range(-0.3f, 0.3f)),
                    transform.rotation, itemParent);
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            }
            obj = Instantiate(itemContainer, transform.position, transform.rotation, itemParent);
            obj.GetComponent<ItemContainer>().item = GameManager.itemsPrefabs[Random.Range(0, GameManager.itemsPrefabs.Length-1)];
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        base.Base(col);
    }
}
