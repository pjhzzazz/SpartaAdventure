using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitMovingPlatform : MonoBehaviour
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
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.transform.SetParent(gameObject.transform);
            StartCoroutine(Moving());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.transform.SetParent(null);

            StartCoroutine(ResetPosition());
        }
    }

    IEnumerator Moving()
    {
        yield return new WaitForSeconds(1f);
        if (go)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                go = false;
                back = true;
            }
        }

        if (back)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, startPos) < 0.01f)
            {
                go = true;
                back = false;
            }
        }
    }
    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(3f);
        transform.position = startPos;
    }
}


