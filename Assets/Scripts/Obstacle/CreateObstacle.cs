using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObstacle : MonoBehaviour
{
    public GameObject[] prefabs;
    public float spawnTime;
    public Transform[] spawnPositions;

    private void Start()
    {
        InvokeRepeating("SpawnObstacle", spawnTime, spawnTime);
    }
    

    public void SpawnObstacle()
    {
        for (int i = 0; i < prefabs.Length; i++)
        Instantiate(prefabs[0], spawnPositions[0].position, Quaternion.identity); 
        Instantiate(prefabs[1], spawnPositions[1].position,Quaternion.Euler(0, 180, 0)); 
    }
}
