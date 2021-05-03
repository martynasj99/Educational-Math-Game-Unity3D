using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour
{

    public Transform ball1;
    public Transform ball2;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
}
