﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float rotationsPerSecond = 0.1f;
    public bool ___________________;
    public int levelShown = 0;
    
	void Update ()
    {
        //read the current level from the hero singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);

        //if this is different from level shown
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            Material mat = this.GetComponent<Renderer>().material;

            //adjust the texture offset to show different shield level
            mat.mainTextureOffset = new Vector2(0.2f*levelShown, 0);
        }

        //rotate the shields a bit every second
        float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
	}
}
