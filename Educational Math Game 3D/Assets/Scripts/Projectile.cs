using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float MAX_VELOCITY = 2000f;

    public float velocity = 10f;

    private GameObject ball;

    public bool isPlayer;

    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance = Screen.height * 0.2f;  //minimum distance for a swipe to be registered

    void Start()
    {
        
    }

    void Update()
    {
        ball = gameObject.GetComponent<WeaponSwitch>().balls[gameObject.GetComponent<WeaponSwitch>().selected];
        if (Input.mousePresent && isPlayer)
        {
            if (Input.GetButtonUp("Fire1") )
            {
                Throw();
            }
            if (Input.GetButton("Fire1") )
            {
                velocity += 3;
                if(velocity > MAX_VELOCITY)
                {
                    velocity = MAX_VELOCITY;
                }
            }
        }
        //Reference : https://forum.unity.com/threads/simple-swipe-and-tap-mobile-input.376160/
        else if(isPlayer)
        {
            if (Input.touchCount == 1) // user is touching the screen with a single touch
            {
                Touch touch = Input.GetTouch(0); // get the touch
                if (touch.phase == TouchPhase.Began) //check for the first touch
                {
                    fp = touch.position;
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
                {
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
                {
                    lp = touch.position;  //last touch position. Ommitted if you use list

                    //Check if drag distance is greater than 20% of the screen height
                    if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                    {//It's a drag
                     //check if the drag is vertical or horizontal
                        if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                        {   //If the horizontal movement is greater than the vertical movement...
                            if ((lp.x > fp.x))  //If the movement was to the right)
                            {   //Right swipe
                                Debug.Log("Right Swipe");
                            }
                            else
                            {   //Left swipe
                                Debug.Log("Left Swipe");
                            }
                        }
                        else
                        {   //the vertical movement is greater than the horizontal movement
                            if (lp.y > fp.y)  //If the movement was up
                            {   //Up swipe
                                Debug.Log("Up Swipe");
                                float diff = lp.y - fp.y;
                                velocity = (diff / Screen.height) * MAX_VELOCITY;
                                Throw();
                            }
                            else
                            {   //Down swipe
                                Debug.Log("Down Swipe");
                            }
                        }
                    }
                    else
                    {   //It's a tap as the drag distance is less than 20% of the screen height
                        Debug.Log("Tap");
                    }
                }
            }
        }
    }

    public void Throw()
    {
        if (!isPlayer)
        {
            velocity = 1000f;
        }
        GameObject _ball = Instantiate(ball, transform.position, transform.rotation);
        _ball.GetComponent<Rigidbody>().AddRelativeForce(ball.transform.forward*velocity);
        if (isPlayer)
        {
            velocity = 10f;
            WindSystem.AddWindable(_ball);
        }
    }
}
