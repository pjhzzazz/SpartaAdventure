using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : MonoBehaviour
{
    public float sensitivityY = 1.5f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float pitch = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        transform.localEulerAngles = new Vector3(-pitch, 0f, 0f);
    }

}
