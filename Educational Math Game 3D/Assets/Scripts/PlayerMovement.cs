using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Reference: https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
 */
public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public float speed = 12f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    public Joystick joystick;

    Vector3 velocity;

    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x, z;

        if (Input.mousePresent)
        {
            joystick.gameObject.SetActive(false);
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            Debug.Log("move");
        }
        else
        {
            joystick.gameObject.SetActive(true);
            x = joystick.Horizontal;
            z = joystick.Vertical;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }
}
