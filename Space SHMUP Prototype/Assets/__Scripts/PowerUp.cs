using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //this is an unusual but handy use of Vector2s. x holds a min value and y a max valuefor a Random.Range() that will be called later
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 6f; //Seconds the Powerup Exists
    public float fadeTime = 4f; // Seconds it will then fade
    public bool ______________;
    public WeaponType type; //the type of the PowerUp
    public GameObject cube; // reference to the cube child
    public TextMesh letter; //Reference to the TextMesh
    public Vector3 rotPerSecond; //Euler rotation speed
    public float birthTime;

    void Awake()
    {
        //find the cube reference
        cube = transform.Find("Cube").gameObject;

        //find the TextMesh
        letter = GetComponent<TextMesh>();

        //set a random velocity
        Vector3 vel = Random.onUnitSphere; //get random xyz velocity
        //Random.onUnitSphere gives you a vector point that is somewhere on the surface of the sphere with 1m radius around the origin

        vel.z = 0; //flatten the vel to the XY plane
        vel.Normalize(); //make the length of the vel 1
        //normalizing a Vector3 makes its length 1

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        //above sets the velocity length to something between x and y of driftMinMax

        GetComponent<Rigidbody>().velocity = vel;

        //set the rotation of this GameObject to R:[0, 0, 0]
        transform.rotation = Quaternion.identity; //Quaternion.identity = no rotation

        //set up the rotPerSecond for the Cube child uaing rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        //check offScreen every 2 seconds
        InvokeRepeating("CheckOffscreen", 2f, 2f);

        birthTime = Time.time;
    }

    void Update()
    {
        //manually rotate the cube child every Update90, multiplying it by Time.time causes the rotation to be time based
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //fade out the powerUp over time. Given the default values, a powerup will exist for ten seconds and then fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //for lifetime seconds, u will be <= 0. then it will transition to 1 over fadetime seconds

        //if U >= 1, destroy this powerup
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //use U to determine the alpha value of the cube and letter
        if (u > 0)
        {
            Color c = cube.GetComponent<Renderer>().material.color;
            c.a = 1f - u;
            cube.GetComponent<Renderer>().material.color = c;

            //fade the letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
    }

    //this SetType() differs from the ones in Weapon and Projectile
    public void SetType(WeaponType wt)
    {
        //grab the weapondefinition from main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);

        //set the color of the cube child
        cube.GetComponent<Renderer>().material.color = def.color;

        //letter.color = def.color; //we could colorize the letter too
        letter.text = def.letter;//set the letter used by the powerup

        type = wt; //finally set the type
    }

    public void AbsorbedBy(GameObject target)
    {
        //this function is called by the hero class when a powerup is collected
        //we could tween intothe target and shrink in size, but for now, just destroy(this.gameObject)
        Destroy(this.gameObject);
    }

    void CheckOffscreen()
    {
        //if this powerup has drifted entirely off screen...
        if (Utils.ScreenBoundsCheck(cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero)
        {
            //...then destroy this gameobject
            Destroy(this.gameObject);
        }
    }
}
