using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //singleton

    //ship movement control
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    //ship status info
    public float shieldLevel = 1;

    public bool ___________________;

    private void Awake()
    {
        S = this; //setting singleton
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

        //rotate ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);
	}
}
