using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Reference: https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
 */
public class MouseLook : MonoBehaviour{

    public float mouseSpeed = 200f;
    public Transform playerBody;

    private float xRotation = 0f;

    public Joystick joystick;
    void Start()
    {
        Cursor.lockState = Input.mousePresent ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void Update()
    {
        float mouseX, mouseY;
        if (Input.mousePresent)
        {
            joystick.gameObject.SetActive(false);
            mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        }
        else
        {
            joystick.gameObject.SetActive(true);
            mouseX = joystick.Horizontal * 100 * Time.deltaTime;
            mouseY = joystick.Vertical * 100 * Time.deltaTime;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
