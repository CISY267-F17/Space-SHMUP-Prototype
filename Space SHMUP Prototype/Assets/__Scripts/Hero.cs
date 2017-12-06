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

    public bool ___________________;

    public Bounds bounds;

    private void Awake()
    {
        S = this; //setting singleton
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
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
