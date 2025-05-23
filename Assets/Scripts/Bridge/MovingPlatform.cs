using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    public float distance;
    public float speed;
    public Vector3 direction;

    private bool go;
    private bool back;
    

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + direction * distance;
        go = true;
        back = false;
    }

    void Update()
    {
        if (go)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.rotation *= Quaternion.Euler(0, 180, 0);
                go = false;
                back = true;
            }
        }

        if (back)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, startPos) < 0.01f)
            {
                transform.rotation *= Quaternion.Euler(0, 180, 0);
                go = true;
                back = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
