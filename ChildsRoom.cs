using UnityEngine;
using System.Diagnostics.CodeAnalysis;

public abstract class ChildsRoom : MonoBehaviour
{

    public static bool kill = false;
    private float nextDelayTime;
    public float life;
    protected bool inFire;

    public virtual void Base()
    {
        if (kill)
            life = 0;
    }

    public virtual void OnFire(float damage, float delayRate)
    {
        if (inFire && nextDelayTime < Time.time)
        {
            life -= damage;
            nextDelayTime = Time.time + delayRate;
        }
    }

    public virtual void Base(Collider2D col)
    {
        //Debug.Log(col.gameObject.name);
        if (col.gameObject.name == "Fire1(Clone)" || col.gameObject.name == "Fire2(Clone)")
        {
            life--;
            Destroy(col.gameObject);
        }
        if (col.gameObject.name == "FireBarrel")
        {
            inFire = true;
        }
    }
}
