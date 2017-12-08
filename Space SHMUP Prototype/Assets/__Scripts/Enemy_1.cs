using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy_1 extends the enemy class
public class Enemy_1 : Enemy
{
    //Because Enemy_1 extends enemy, the bool______ wont work the same way in the Inspector pane

    //#of seconds for a full sine wave
    public float waveFrequency = 2;
    //sine wave width in meters
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0 = -12345; //the initial x value of pos
    private float birthTime;

	// Use this for initialization
	void Start ()
    {
        //set x0 to the initial position of Enemy_1
        x0 = pos.x;

        birthTime = Time.time;
	}

    //override the move() function on enemy
    public override void Move()
    {
        //pos is property. can't directly set. cheat as Vector3
        Vector3 tempPos = pos;

        //theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //base Move() still handles the movement down the y axis
        base.Move();
    }
}
