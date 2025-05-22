using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorLaser : MonoBehaviour
{
    public float maxDistance = 20f;
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    public Vector3 direction;
    [SerializeField] private LineRenderer[] laserLines;
    [SerializeField] private Material laserMaterial;
    void Start()
    {
        foreach (LineRenderer laser in laserLines)
        {
            laser.startColor = Color.red;
            laser.endColor = Color.yellow;
            laser.startWidth = 0.2f;
            laser.endWidth = 0.2f;
            laser.positionCount = 2;
            laser.material = laserMaterial;
        }
        
    }
    void Update()
    {
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * 5f;

        for (int i = 0; i < laserLines.Length; i++)
        {
            Vector3 origin = transform.position + transform.up * i * 0.5f;
            Vector3 direction = transform.forward;
            Vector3 endPosition = origin + direction * maxDistance;
            
            laserLines[i].SetPosition(0, origin);
            laserLines[i].SetPosition(1, endPosition);
        }
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            RaycastHit hit;
            Ray[] rays = new Ray[4]
            {
                new Ray(transform.position   ,direction),
                new Ray(transform.position + (transform.up * 1f), direction),
                new Ray(transform.position + (transform.up * 1.5f), direction),
                new Ray(transform.position + (transform.up * 0.5f), direction)
            };
        
            for(int i = 0; i < rays.Length; i++)
            {
                if(Physics.Raycast(rays[i], out hit, maxCheckDistance, layerMask))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Debug.Log("플레이어 감지!");
                    }
                }
            }
        }
    }

}
