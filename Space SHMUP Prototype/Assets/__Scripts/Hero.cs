using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //singleton

    public float gameRestartDelay = 2f;

    //ship movement control
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    //ship status info
    [SerializeField]
    public float _shieldLevel = 1;

    //Weapon fields
    public Weapon[] weapons;

    public bool ___________________;

    public Bounds bounds;

    //declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //create a WeaponFireDelegate field named fireDelegate
    public WeaponFireDelegate fireDelegate;

    void Awake()
    {
        S = this; //setting singleton
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }

    void Start()
    {
        //reset the weapons to start the hero with one blaster
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    void Update ()
    {
        //pull info from input class
        float xAxis = Input.GetAxis("Horizontal");  //1
        float yAxis = Input.GetAxis("Vertical");    //1

        //change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        bounds.center = transform.position;

        //Keep the ship contained to the screen bounds
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
        if (off != Vector3.zero)
        {
            pos -= off;
            transform.position = pos;
        }

        //rotate ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);

        //use the fireDelegate to fire weapons
        //First , make sure the Axis("Jump") button is pressed, then, ensure that fireDelegate isn't null to avoid an error
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
	}

    //this variable holds a reference to the last triggering GameObject
    public GameObject lastTriggerGo = null;

    void OnTriggerEnter(Collider other)
    {
        //find the tag of the other.gameObject or its parent GameObject
        GameObject go = Utils.FindTaggedParent(other.gameObject);

        //if there is a parent with a tag
        if (go != null)
        {
            //make sure it's not the same triggering go as last time
            if (go == lastTriggerGo)
            {
                return;
            }

            lastTriggerGo = go;

            if (go.tag == "Enemy")
            {
                //if the shield was triggered by the enemy, decrease the shield level by 1
                shieldLevel--;

                //destroy the enemy
                Destroy(go);
            }

            else if (go.tag == "PowerUp")
            {
                //if the shield was triggered by a powerup
                AbsorbPowerUp(go);
            }

            else
            {
                print("Triggered: " + go.name);
            }
        }
        else
        {
            //otherwise announce the original other.gameObject
            print("Triggered: " + other.gameObject.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield: //if its a shield
                shieldLevel++;
                break;

            default: //if its any weapon powerup
                //check the current weaponType
                if (pu.type == weapons[0].type)
                {
                    //then increase the number of weapons of this type
                    Weapon w = GetEmptyWeaponSlot(); //find an avaliable weapon
                    if (w != null)
                    {
                        //set it to pu.type
                        w.SetType(pu.type);
                    }
                }

                else
                {
                    //if this is a different weapon
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }

        set
        {
            _shieldLevel = Mathf.Min(value, 4);

            //if the shield is going to be set to less than zero
            if (value < 0)
            {
                Destroy(this.gameObject);

                //tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
