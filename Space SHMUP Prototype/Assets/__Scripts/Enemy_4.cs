using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    //these three have to be defined in the inspector pane
    public string name; //name of part
    public float health; //hp of part
    public string[] protectedBy; //other parts that protect this

    //set automatically in Start()
    public GameObject go; //the GO of this part
    public Material mat; //Mat to show damage
}

public class Enemy_4 : Enemy
{
    //start off screen then follow random points
    public Vector3[] points;
    public float timeStart;
    public float duration = 4;

    public Part[] parts; //array of ship parts

	// Use this for initialization
	void Start ()
    {
        points = new Vector3[2];
        //initial position pre-set
        points[0] = pos;
        points[1] = pos;

        InitMovement();

        //Cache GameObject & Material of each part in parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
	}

    void InitMovement()
    {
        //pick point on screen
        Vector3 p1 = Vector3.zero;
        float esp = Main.S.enemySpawnPadding;
        Bounds cBounds = Utils.camBounds;
        p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
        p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

        points[0] = points[1]; //shift points
        points[1] = p1; //add p1 as a point

        //reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        //completely override with linear interpolation

        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            u=0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); //easing out

        pos = (1 - u) * points[0] + u * points[1]; //simple linear interpolation
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //no damage til on screen
                bounds.center = transform.position + boundsCenterOffset;
                if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                {
                    Destroy(other);
                    break;
                }

                //hurt the enemy, check what was hit, 
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    //muscle past a potential collider error
                    goHit = coll.contacts[0].thisCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                //check if part is still protected
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        //if one of the protecting parts hasn'y been destroyed yet..
                        if (!Destroyed(s))
                        {
                            //...then don't damage this part yet
                            Destroy(other); //destroy the projectile
                            return; //return before causing damage
                        }
                    }
                }

                //not protected = take damage. get amount from Projectile.type & Main.W_DEFS
                prtHit.health -= Main.W_DEFS[p.type].damageOnHit;

                //show damage on the part
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    //instead of destroying the part, disable it
                    prtHit.go.SetActive(false);
                }

                //check to see if the whole ship is destroyed
                bool allDestroyed = true; //assume true
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt)) //if any parts still exist, set allDestroyed to false
                    {
                        allDestroyed = false;
                        break;
                    }
                }

                if (allDestroyed) //if it is destroyed, tell the main singleton and destroy
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }

                Destroy(other); //Destroy the Projectile Hero
                break;
        }
    }

    //these two functions find a Part in this.parts by name or GO
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    //these functiond return true if the part has been destroyed
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if (prt == null) //if nothing was passed in, it must have been destroyed
        {
            return (true);
        }
        return (prt.health <= 0); //confirm and set part destroyed condition
    }

    //this changes the color of just one part to red instead of the whole thing when damage is applied
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        remainingDamageFrames = showDamageForFrames;
    }
}
