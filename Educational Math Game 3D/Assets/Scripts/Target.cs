using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject explode;

    public int value;
    public float health = 500f;

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.Find("QuizManager").GetComponent<QuizManager>().ProvideNumber(value);
        gameObject.SetActive(false);
        GameObject explodeParticle = Instantiate(explode, transform.position, Quaternion.identity);
        GameObject.Destroy(explodeParticle, 1.0f);
        Destroy(gameObject, 3.0f);
        health = 1000f;
        Invoke("Respawn", 2.0f);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        Instantiate(gameObject, transform.position, transform.rotation);
    }
    
    public void TakeDamage(float amount)
    {
        //gameObject.GetComponent<Rigidbody>().AddForce(direction);
        //Invoke("Die", 1.0f);
        health -= amount;
    }

    public void ApplyModifier(int modifier)
    {
        value = Mathf.Abs(value) * modifier;
    }
}
