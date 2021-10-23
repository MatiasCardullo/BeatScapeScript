using UnityEngine;

public class Barrel : ChildsRoom
{
    public CircleCollider2D fire;
    private bool exploded = false;
    private float delayFire =0.1f, nextDelayFire;

    void Start()
    {
        life = 10;
    }

    void Update()
    {
        base.Base();
        base.OnFire(0.1f, 0.2f);
        if (life <= 0)
        {
            foreach (BoxCollider2D i in GetComponents<BoxCollider2D>())
                { i.enabled = false; }
            if (!exploded && fire.radius < 1)
            {
                fire.radius += 0.007f;
            }
            else
            {
                exploded = true;
            }
            if (exploded)
            {
                if (nextDelayFire < Time.time)
                {
                    fire.radius -= 0.004f;
                    nextDelayFire = Time.time + delayFire;
                }
                else if (fire.radius < 0.004f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        base.Base(col);
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name == "FireBarrel")
        {
            inFire = false;
        }
    }
}
