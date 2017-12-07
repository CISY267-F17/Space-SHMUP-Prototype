﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main S; //singleton
    public static Dictionary<WeaponType, WeaponDefinition> W_DEFS;

    public GameObject[] prefabEnemies; //an array of enemies
    public float enemySpawnPerSecond = 0.5f; //# enemies/sec
    public float enemySpawnPadding = 1.5f; //Padding for position
    public WeaponDefinition[] weaponDefinitions;

    public bool ____________________;

    public WeaponType[] activeWeaponTypes;
    public float enemySpawnRate; //delay between Enemy spawns

	// Use this for initialization
	void Awake ()
    {
        S = this;

        //set Utils.camBounds
        Utils.SetCameraBounds(this.GetComponent<Camera>());

        //0.5 enemies/sec = enemySpawnRate of 2
        enemySpawnRate = 1f / enemySpawnPerSecond;

        //Invoke call SpawnEnemy() once after a 2 second delay
        Invoke("SpawnEnemy", enemySpawnRate);

        //a generic dictionary with WeaponType as the key
        W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            W_DEFS[def.type] = def; 
        }
	}

    public static WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //check to make sure that the key exists in the Dictionary
        //attemption to retrieve a key that didnt exist would throw an error, so the following if statement is important
        if (W_DEFS.ContainsKey(wt))
        {
            return (W_DEFS[wt]);
        }

        //this will return a definition for WeaponType.none, which means it has failed to find the WeaponDefinition
        return (new WeaponDefinition());
    }

    void Start()
    {
        activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
        for (int i = 0; i < weaponDefinitions.Length; i++)
        {
            activeWeaponTypes[i] = weaponDefinitions[i].type;
        }
    }

    public void SpawnEnemy()
    {
        //Pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;

        //Position the enemy above the screen with a random x position
        Vector3 pos = Vector3.zero;
        float xMin = Utils.camBounds.min.x + enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - enemySpawnPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = Utils.camBounds.max.y + enemySpawnPadding;
        go.transform.position = pos;

        //call SpawnEnemy() again in a couple of seconds
        Invoke("SpawnEnemy", enemySpawnRate);
    }

    public void DelayedRestart(float delay)
    {
        //Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        //Reload _Scene_0 to restart the game
        Application.LoadLevel("__Scene_0");
    }
}
