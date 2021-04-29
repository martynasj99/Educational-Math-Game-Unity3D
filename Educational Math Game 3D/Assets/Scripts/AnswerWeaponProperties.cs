using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerWeaponProperties : MonoBehaviour
{

    public int modifier;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Answer")
        {
            Target target = collision.gameObject.GetComponent<Target>();
            target.ApplyModifier(modifier);
            collision.gameObject.GetComponent<Rigidbody>().AddRelativeForce(gameObject.transform.forward * GameObject.Find("WeaponHolder").GetComponent<Projectile>().velocity);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 4f);
        }
    }
}
