using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Reference: https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
 */
public class MouseLook : MonoBehaviour{

    public float mouseSpeed = 1000f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        
        float mouseX = Input.GetAxis("Mouse X") != 0 ?
            Input.GetAxis("Mouse X")  : Input.GetAxis("Joy X");
        mouseX *= mouseSpeed * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") != 0 ?
            Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime : 
            Input.GetAxis("Joy Y") * mouseSpeed * Time.deltaTime * -1;
   
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
