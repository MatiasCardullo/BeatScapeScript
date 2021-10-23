using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour 
{
    public Inventory inventory;
    public Slider slider; public Gradient gradient;
    public Image healthBar, shieldBar, shieldIcon, shieldIcon2;

    public Animator bottomAnimator;
    public GameObject topAnimator, topAnimator2;
    public GameObject levelText, coinText;
    public GameObject crossHair, firePrefab;

    public byte forceLevel = 0,maxlevel;
    public float fireRate;
    public bool admin=false;

    public int coin = 0, speed, basespeed;
    public static byte level = 0;
    public static ushort shield, life, life2;
    public static ushort maxShield, maxLife, maxLife2;

    private Image shieldStat;
    private bool iconOrBar, facingRight;
    private float hMove = 0, vMove = 0, delayShield, delayStair;
    private double nextDelayShield, nextDelayStair, nextFireTime;
    private Item itemPicked;
    private Vector2 movement,shoot;


    void Start()
    {
        maxlevel = GameManager.maxlevel;
        transform.position = new Vector2(0,-2);
        shield = 0; maxShield = 100;
        life = 100; maxLife = life;
        life2 = 20; maxLife2 = life2;
        if (admin)
            delayShield = 0.01f;
        else
            delayShield = 1f;
        delayStair = 0.5f;
        basespeed = 250;
        speed = basespeed;
        fireRate = 0.1f;
        crossHair.SetActive(true);
        shoot = new Vector2(-0.5f, 0.3f);
        crossHair.transform.localPosition= shoot;
        if (iconOrBar)
        {
            shieldBar.gameObject.SetActive(true);
            shieldStat = shieldBar;
            shieldIcon2.gameObject.SetActive(false);
        }
        else
        {
            shieldIcon2.gameObject.SetActive(true);
            shieldStat = shieldIcon;
            shieldBar.gameObject.SetActive(false);
        }
        SetMaxHealth();
        SetHealthBar();
        coinText.GetComponent<Text>().text = coin.ToString();
    }

    void Update()
    {
        InputManager();
        if (forceLevel != level && forceLevel <= maxlevel)
            level = forceLevel;
        if (shield < maxShield && nextDelayShield < Time.time)
        {
            shield++;
            nextDelayShield = Time.time + delayShield;
        }
        levelText.GetComponent<Text>().text = " LEVEL " + level;
        SetHealthBar();
        movement = new Vector2(hMove, vMove);
        transform.position += (Vector3)movement * speed*0.01f * Time.deltaTime;
        bottomAnimator.SetFloat("Horizontal", hMove);
        bottomAnimator.SetFloat("Vertical",vMove);
        if (hMove > 0 && !facingRight)
            Flip();
        else if (hMove < 0 && facingRight)
            Flip();
        if (movement.magnitude > 0.2f)
		{
            shoot = movement;
            shoot.Normalize();
            shoot *= 0.5f;
            crossHair.transform.localPosition = new Vector2(shoot.x,shoot.y+0.3f);
        }
        if (life2 <= 0)
        {
            transform.position = new Vector2(20, 200);
            //GameObject.FindGameObjectWithTag("CameraTarget").transform.position = transform.position;
            coinText.GetComponent<Text>().text = "Game Over";
            gameObject.SetActive(false);
        }
    }

    private void InputManager()
    {
        if (admin)
        {
            if (Input.GetKeyDown("k"))
            {
                Enemy.kill = true;
                Crate.kill = true;
            }
            if (Input.GetKeyUp("k"))
            {
                coin += GameObject.FindGameObjectsWithTag("Coin").Length;
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Coin"))
                {
                    Destroy(item);
                }
                Enemy.kill = false;
                Crate.kill = false;
            }
            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                if (forceLevel == 0)
                {
                    forceLevel = maxlevel;
                }
                else
                {
                    forceLevel--;
                }
            }
            else if (Input.GetKeyUp(KeyCode.PageUp))
            {
                if (forceLevel >= maxlevel)
                {
                    forceLevel = 0;
                }
                else
                {
                    forceLevel++;
                }
            }
        }
        if (life>0)
        {
            hMove = Input.GetAxis("Horizontal");
            vMove = Input.GetAxis("Vertical");
            if ((hMove > 0 || hMove < 0 || vMove > 0 || vMove < 0)
                && Input.GetButton("Fire1") && speed < basespeed * 2)
            {
                speed += 1;
            }
            else if (speed > basespeed)
            {
                speed -= 1;
            }
            if (Input.GetButton("Fire3"))
            {
                Shooting(shoot);
            }
        }
        else
        {
            hMove = 0; vMove = 0;
        }
    }

    private void Flip()
    {
        Vector3 theScale; facingRight = !facingRight;
        theScale = topAnimator2.transform.localScale;
        theScale.x *= -1;
        topAnimator2.transform.localScale = theScale;
        theScale = topAnimator.transform.localScale;
        theScale.x *= -1;
        topAnimator.transform.localScale = theScale;
    }

    private void Shooting(Vector2 shootVector)
    {
        if (nextFireTime < Time.time)
        {
            GameObject fire = Instantiate(firePrefab, crossHair.transform.position, Quaternion.identity);
            fire.GetComponent<Rigidbody2D>().velocity = shootVector * 6 * speed*0.01f;
            fire.transform.Rotate(0f, 0f, Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg);
            nextFireTime = Time.time + fireRate;
        }
    }
	
	public void SetMaxHealth()
    {
        slider.maxValue = maxLife + maxLife2;
        healthBar.color = gradient.Evaluate(1f);
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.red;
        colorKey[0].time = (float)maxLife2 / (maxLife + maxLife2);
        colorKey[1].color = Color.yellow;
        colorKey[1].time = (float)maxLife2 / (maxLife + maxLife2) + ((float)maxLife / (maxLife + maxLife2))*0.5f;
        colorKey[2].color = Color.green;
        colorKey[2].time = 1.0f;
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        gradient.SetKeys(colorKey,alphaKey);
    }

    public void SetHealthBar()
    {
        slider.value = life + life2;
        healthBar.color = gradient.Evaluate(slider.normalizedValue);
        shieldStat.fillAmount = (float)shield / maxShield;
    }

    private void LifeManager(ushort descount)
    {
        if (shield > 0)
        {
            if (shield < descount)
            {
                descount -= shield;
                shield = 0;
            }
            else
            {
                shield -= descount;
                return;
            }
        }
        if (life > 0)
        {
            if (life < descount)
            {
                descount -= life;
                life = 0;
            }
            else
            {
                life -= descount;
                return;
            }
        }
        if (life2 > 0)
        {
            if (life2 < descount)
                life2 = 0;
            else
                life2 -= descount;
        }
    }

    void OnApplicationQuit()
    {
        if (!admin)
			inventory.Container.Clear();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Coin"))
        {
            Destroy(col.gameObject, 0.1f);
            coin++;
            coinText.GetComponent<Text>().text = coin.ToString();
        }
        else if (col.CompareTag("Item"))
        {
            if(itemPicked != col.GetComponent<ItemContainer>().item)
            {
                itemPicked = col.GetComponent<ItemContainer>().item;
                inventory.AddItem(itemPicked, 1);
                Destroy(col.gameObject);
            }
        }
        else
        {
            if (col.gameObject.name == "Fire2(Clone)")
            {
                Destroy(col.gameObject);
                LifeManager(5);
            }
            if (nextDelayStair < Time.time)
            {
                if (col.gameObject.name == "StairUp" && level < 100)
                {
                    level++; forceLevel++;
                    transform.position = new Vector2(0f, 1.7f);
                    nextDelayStair = Time.time + delayStair;
                }
                else if (col.gameObject.name == "StairDown" && level > 0)
                {
                    level--; forceLevel--;
                    transform.position = new Vector2(0f, -1.2f);
                    nextDelayStair = Time.time + delayStair;
                }
            }
        }
    }
}
