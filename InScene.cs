
using UnityEngine;

public class InScene : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject tile;
    public Rigidbody2D rgbody;
    public Collider2D colision;

    void OnTriggerEnter2D(Collider2D col)
    {
        if ("CameraTarget" == col.gameObject.name)
        {
            if (sprite != null)
                sprite.enabled = true;
            if (tile != null)
                tile.SetActive(true);
			if (rgbody != null)
                rgbody.WakeUp();
            if (colision != null)
                colision.enabled = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if ("CameraTarget" == col.gameObject.name)
        {
            if (sprite != null)
                sprite.enabled = false;
            if (tile != null)
                tile.SetActive(false);
			if (rgbody != null)
                rgbody.Sleep();
            if (colision != null)
                colision.enabled = false;
        }
    }
}
