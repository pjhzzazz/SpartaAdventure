using UnityEngine;

public class OneWayMove : MonoBehaviour
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
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}