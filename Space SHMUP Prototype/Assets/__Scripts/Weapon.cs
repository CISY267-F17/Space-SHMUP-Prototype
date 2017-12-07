using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is an enum of the various possible weapon types
//it has also included a shield type to allow a shield power-up
//items marked [NI] are not implimented in this example
public enum WeaponType
{
    none, //default, no weapon
    blaster, //a simple blaster
    spread, //two shots simultaneously
    phaser, //shots that move in waves[NI]
    missile, //homing missles[NI]
    laser, //damage over time[NI]
    shield, //raises shieldLevel
}

//the WeaponDefinition class allows you to set the properties of a specific weapon in the inspector
//Main has an array of WeaponDefinitions that makes this possible
//[System.Serializable] tells Unity to try to view WeaponDefinition in the Inspector pane
//It doesn't always work for everything, but it will work for simple classes like this
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //the letter to show on the power-up
    public Color color = Color.white; //color of the Collar and power-up
    public GameObject projectilePrefab; //prefab for projectiles
    public Color projectileColor = Color.white; //projectile color
    public float damageOnHit = 0; //amount of damage caused
    public float continuousDamage = 0; //damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; //speed of projectiles
}

//NOTE: Weapon prefabs, colors, and so on are set in the Main class.

public class Weapon : MonoBehaviour
{
    public static Transform PROJECTILE_ANCHOR;

    public bool ____________________;
    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot; // time last shot was fired

    void Awake()
    {
        collar = transform.Find("Collar").gameObject;
    }
    void Start()
    {
        //call SetType() properly for the default _type
        SetType(_type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        //find the fireDelegate of the parent
        GameObject parentGO = transform.parent.gameObject;
        if (parentGO.tag == "Hero")
        {
            Hero.S.fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get
        {
            return (_type);
        }

        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }

        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collar.GetComponent<Renderer>().material.color = def.color;
        lastShot = 0; //you can always fire immediately after _type is set
    }

    public void Fire()
    {
        //if this gameobject is inactive, return
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        //if it hasn't been enough time between shots, return
        if (Time.time - lastShot < def.delayBetweenShots)
        {
            return;
        }

        Projectile p;
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(-0.2f, 0.9f, 0) * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(0.2f, 0.9f, 0) * def.velocity;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate(def.projectilePrefab) as GameObject;
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }

        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = collar.transform.position;
        go.transform.parent = PROJECTILE_ANCHOR;
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return (p);
    }
}
