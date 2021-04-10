using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int value;
    public float health = 500f;

    void Update()
    {
        if (health <= 0)
        {
            GameObject.Find("QuizManager").GetComponent<QuizManager>().ProvideNumber(value);
            gameObject.SetActive(false);
            Destroy(gameObject, 3.0f);
            health = 1000f;
            Invoke("Respawn", 2.0f);
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        Instantiate(gameObject, transform.position, transform.rotation);
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void ApplyModifier(int modifier)
    {
        value = Mathf.Abs(value) * modifier;
    }
}
